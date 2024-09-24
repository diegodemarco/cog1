#include <stdio.h>
#include <time.h>
#include <errno.h> 
#include <fcntl.h>
#include <stdint.h>
#include <unistd.h>
#include <gpiod.h>
#include <pthread.h>
#include "../include/cog1_i2c.h"
#include "../include/cog1_oled.h"

#define io_expander_address_1 			0x38
#define io_expander_address_2 			0x20
#define adc_address						0x1d

// Number of input pins monitored via IRQ
#define IRQ_PIN_COUNT					7

// Encoder pins
#define PIN_ENCODER_A					72
#define PIN_ENCODER_B					231
#define PIN_ENCODER_BUTTON				75

// Heartbeat LED pin
#define PIN_HEARTBEAT					73

// Digital input pins
#define PIN_DI_1						74
#define PIN_DI_2						233
#define PIN_DI_3						230
#define PIN_DI_4						232

// IO events
#define IO_EVENT_ENCODER_CW				1
#define IO_EVENT_ENCODER_CCW			2
#define IO_EVENT_ENCODER_SW_ACTIVE		4
#define IO_EVENT_ENCODER_SW_INACTIVE	8
#define IO_EVENT_DI1_ACTIVE				16
#define IO_EVENT_DI1_INACTIVE			32
#define IO_EVENT_DI2_ACTIVE				64
#define IO_EVENT_DI2_INACTIVE			128
#define IO_EVENT_DI3_ACTIVE				256
#define IO_EVENT_DI3_INACTIVE			512
#define IO_EVENT_DI4_ACTIVE				1024
#define IO_EVENT_DI4_INACTIVE			2048

const char *i2c_device = "/dev/i2c-3";

int oled_fd = 0;
int expander_fd = 0;
int adc_fd = 0;

pthread_t _h_di_thread;
unsigned char io_expander_address = 0;
char *_IO_CHIP_NAME = "gpiochip0";
char *_IO_CONSUMER = "cog1";
void (*_di_isr)(int event_bitmap) = NULL;
unsigned int _io_line_di_offsets[IRQ_PIN_COUNT] = { PIN_ENCODER_A, PIN_ENCODER_B, PIN_ENCODER_BUTTON, PIN_DI_1, PIN_DI_2, PIN_DI_3, PIN_DI_4 };

struct gpiod_chip *_h_io_chip = NULL;
struct gpiod_line_bulk _h_io_line_bulk_di;
struct gpiod_line *_h_line_heartbeat;

int heartbeat(int x)
{
	return gpiod_line_set_value(_h_line_heartbeat, x ? 0 : 1);		// Low value = ON
}

int do_control( int do_bitmap )
{
	// P7 in the port expander must be kept low, to signal that the
	// expander has been initialized. This is useful to understand
	// upon startup, if this is a reboot/restart, or if power was
	// lost.
	unsigned char portValue = 0b01111111;			// "active" bits should be zero
	if (do_bitmap & 1) portValue &= 0b11110111;		// DO1 = expander bit 3
	if (do_bitmap & 2) portValue &= 0b11111011;		// DO2 = expander bit 2
	if (do_bitmap & 4) portValue &= 0b11011111;		// DO3 = expander bit 5
	if (do_bitmap & 8) portValue &= 0b11101111;		// DO4 = expander bit 4
	int result = i2c_write_byte(expander_fd, io_expander_address, portValue);
	if (result != 1)
		printf("Error %d setting port expander output to %d\n", result, portValue);
	return (result == 1);
}

int adc_read_channel(unsigned char reg_address, int *value)
{
	unsigned char buffer[2];
	int r = i2c_read_bytes_register(adc_fd, adc_address, reg_address, buffer, 2);
	if (r < 0) return r;
	
	*value = ((int)(buffer[0]) * 0x100) + (int)(buffer[1]);
	*value = *value >> 4;
	
	return r;
}

int adc_read( int *c1, int *c2, int *c3, int *c4, int *c5, int *c6, int *c7, int *c8 )
{
	int r        = adc_read_channel(0x20, c1);
	if (r > 0) r = adc_read_channel(0x21, c2);
	if (r > 0) r = adc_read_channel(0x22, c3);
	if (r > 0) r = adc_read_channel(0x23, c4);
	if (r > 0) r = adc_read_channel(0x24, c5);
	if (r > 0) r = adc_read_channel(0x25, c6);
	if (r > 0) r = adc_read_channel(0x26, c7);
	if (r > 0) r = adc_read_channel(0x27, c8);
	if (r <= 0)
	{
		printf("Error reading adc: %d\n", r);
	}
	return (r > 0);
}

int display_init()
{
	int r = oled_config(oled_fd, 128, 64, 1);
	if (r <= 0) 
	{
		printf("Error initializing OLED display: %d\n", r);
	}
	return (r > 0);
}

void display_clear()
{
	oled_clear(oled_fd);
}

void display_goto_xy(int x, int y)
{
	oled_set_xy(oled_fd, x, y);
}

void display_draw_bitmap(int x, int y, const unsigned char *data, int data_len)
{
	oled_set_xy(oled_fd, x, y);
	oled_show_bitmap(oled_fd, data, data_len);
}

void *_di_poll_thread(void *args)
{
	static int prev_a = 0;
	static int prev_b = 0;
	static int prev_sw = 0;
	static int prev_di1 = 0;
	static int prev_di2 = 0;
	static int prev_di3 = 0;
	static int prev_di4 = 0;
	static int values[IRQ_PIN_COUNT];
	const struct timespec ts = {1, 0};
	static struct gpiod_line_bulk event_lines;
	static struct gpiod_line_event dummy_event;

	for (;;)
	{
		if (gpiod_line_event_wait_bulk(&_h_io_line_bulk_di, &ts, &event_lines) > 0)
		{
			// Read events to reset the wait state of the respective lines
			for (int i = 0; i < gpiod_line_bulk_num_lines(&event_lines); i++)
				gpiod_line_event_read(gpiod_line_bulk_get_line(&event_lines, i), &dummy_event);
			
			int flags = 0;
			
			// Read pin values
			if (gpiod_line_get_value_bulk(&_h_io_line_bulk_di, values) == 0)
			{
				// Encoder-related pins
				if (values[0] != prev_a || values[1] != prev_b || values[2] != prev_sw) 
				{
					//printf("Encoder: A=%d, B=%d, SW=%d\n", values[0], values[1], values[2]);
					
					// Add ISR events if the knob was turned
					if (values[0] && values[1])
					{
						if (prev_a && !prev_b)
						{
							flags |= IO_EVENT_ENCODER_CW;
						} else if (!prev_a && prev_b)
						{
							flags |= IO_EVENT_ENCODER_CCW;
						}
					}
					prev_a = values[0];
					prev_b = values[1];
									
					// Add ISR events if the button changed
					if (values[2] != prev_sw)
						flags |= (values[2] ? IO_EVENT_ENCODER_SW_INACTIVE : IO_EVENT_ENCODER_SW_ACTIVE);
					prev_sw = values[2];
				}
				
				// Digital-input-related pins
				if (values[3] != prev_di1 || values[4] != prev_di2 || values[5] != prev_di3 || values[6] != prev_di4)
				{
					//printf("DI: 1=%d, 2=%d, 3=%d, 4=%d\n", values[3], values[4], values[5], values[6]);
					
					if (values[3] != prev_di1)
						flags |= (values[3] ? IO_EVENT_DI1_INACTIVE : IO_EVENT_DI1_ACTIVE);
					if (values[4] != prev_di2)
						flags |= (values[4] ? IO_EVENT_DI2_INACTIVE : IO_EVENT_DI2_ACTIVE);
					if (values[5] != prev_di3)
						flags |= (values[5] ? IO_EVENT_DI3_INACTIVE : IO_EVENT_DI3_ACTIVE);
					if (values[6] != prev_di4)
						flags |= (values[6] ? IO_EVENT_DI4_INACTIVE : IO_EVENT_DI4_ACTIVE);
					
					prev_di1 = values[3];
					prev_di2 = values[4];
					prev_di3 = values[5];
					prev_di4 = values[6];
				}

			}

			// Call the ISR if there are any events
			if (flags > 0)
			{
				//printf("Calling ISR(%d)\n", flags);
				_di_isr(flags);
			}
			
		}
	}
}

int _di_init()
{
	return
		(gpiod_chip_get_lines(_h_io_chip, _io_line_di_offsets, IRQ_PIN_COUNT, &_h_io_line_bulk_di) == 0)
		&& (gpiod_line_request_bulk_both_edges_events_flags(&_h_io_line_bulk_di, _IO_CONSUMER, GPIOD_LINE_REQUEST_FLAG_BIAS_PULL_UP) == 0);
}

int io_init(void (*di_isr)(int))
{
	int r;
	int retVal = 1;
	unsigned char portStatus;
	unsigned char adc_manufacturer, adc_revision;

	// Open i2c device for each chip
	oled_fd = i2c_open(i2c_device);
	if (oled_fd < 0)
	{
		printf("Error opening oled i2c: %d\n", oled_fd);
		return -1;
	}

	expander_fd = i2c_open(i2c_device);
	if (expander_fd < 0)
	{
		printf("Error opening expander i2c: %d\n", expander_fd);
		return -1;
	}
	
	adc_fd = i2c_open(i2c_device);
	if (adc_fd < 0)
	{
		printf("Error opening adc i2c: %d\n", adc_fd);
		return -1;
	}

	// Check the status of the port expander. If all bits are set to
	// 1, then it means that power was lost since the last time this
	// function was called. The return value of this function will
	// indicate so.
	r = i2c_read_bytes(expander_fd, io_expander_address_1, &portStatus, 1);
	if (r == 1)
	{
		io_expander_address = io_expander_address_1;
	}
	else 
	{
		r = i2c_read_bytes(expander_fd, io_expander_address_2, &portStatus, 1);
		if (r == 1) 
		{
			io_expander_address = io_expander_address_2;
		}
		else 
		{
			printf("i/o expander error at i2c_read_bytes(): %d\n", r);
			return -2;
		}
	}
	printf("i/o expander found at address %d, port value is %d\n", io_expander_address, portStatus);

	if (portStatus == 0xff)
	{
		// The io expander has already been initialized, therefore
		// we are restarting / rebooting, but we haven't lost power
		// since the last execution
		retVal = 0;
	}

	// Lower P7 in the port expander. This pin is not used as an output,
	// but we're using it instead as a witness for the "power lost"
	// condition, because all port pins are set to 1 when the io expander
	// is applied power.
	r = i2c_write_byte(expander_fd, io_expander_address, portStatus & 0b01111111);
	if (r != 1)
	{
		printf("i/o expander error at i2c_write_byte(): %d\n", r);
		return -3;
	}
	
	// Initialize ADC
	r = i2c_write_byte_register(adc_fd, adc_address, 0x00, 0b10000000);			// Reset to default configuration and shut down
	if (r != 1) 
	{
		printf("adc error at i2c_write_bytes_register(0x00->0x80): %d\n", r);
		return -7;
	}

	r = i2c_write_byte_register(adc_fd, adc_address, 0x0b, 0b00000010);			// MODE 1, internal VREF
	if (r != 1)
	{
		printf("adc error at i2c_write_bytes_register(0x0b): %d\n", r);
		return -7;
	}
	
	r = i2c_write_byte_register(adc_fd, adc_address, 0x07, 0b00000001);			// Continuous conversion mode
	if (r != 1)
	{
		printf("adc error at i2c_write_bytes_register(0x07): %d\n", r);
		return -7;
	}

	r = i2c_read_bytes_register(adc_fd, adc_address, 0x3e, &adc_manufacturer, 1);
	if (r != 1) 
	{
		printf("adc error at i2c_read_bytes_register(0x3e): %d\n", r);
		return -7;
	}

	r = i2c_read_bytes_register(adc_fd, adc_address, 0x3f, &adc_revision, 1);
	if (r != 1) 
	{
		printf("adc error at i2c_read_bytes_register(0x3f): %d\n", r);
		return -7;
	}

	if (!i2c_write_byte_register(adc_fd, adc_address, 0x00, 0b00000001)) 		// Start
	{
		printf("adc error at i2c_write_bytes_register(0x00->1)\n");
		return -7;
	}

	printf("adc initialized at address %d, manufacturer=%d, revision=%d\n", adc_address, adc_manufacturer, adc_revision);

	// Open gpio chip
	_h_io_chip = gpiod_chip_open_by_name(_IO_CHIP_NAME);
	if (!_h_io_chip) 
		return -6;
	
	// Open i/o output pins
	_h_line_heartbeat = gpiod_chip_get_line(_h_io_chip, PIN_HEARTBEAT);
	if (!_h_line_heartbeat)
		return -6;
	gpiod_line_request_output(_h_line_heartbeat, _IO_CONSUMER, 1);
	
	// If an ISR was provided for the digial inputs, set up monitoring on the relevant pins
	if (di_isr)
	{
		if (!_di_init())
			return -4;
		
		_di_isr = di_isr;
		if (pthread_create(&_h_di_thread, NULL, &_di_poll_thread, NULL) != 0) 
			return -5;
	}

	// Done
	return retVal;
}

int io_deinit()
{
	heartbeat(1);
	return 1;
}
