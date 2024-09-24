#include <string.h>
#include "../include/ssd1306.h"
#include "../include/cog1_i2c.h"

int _oled_line_count = 0;
int _oled_col_count  = 0;
unsigned char _oled_data_buf[4096];

int oled_config(int fd, int oled_lines, int oled_columns, int rotate)
{
    if (oled_lines != SSD1306_128_64_LINES && oled_lines != SSD1306_128_32_LINES && SSD1306_64_48_LINES)
        oled_lines = SSD1306_128_64_LINES;
        
    if (oled_columns != SSD1306_128_64_COLUMNS && oled_lines != SSD1306_128_32_COLUMNS && SSD1306_64_48_COLUMNS)
        oled_columns = SSD1306_128_64_COLUMNS;
        
    _oled_line_count = oled_lines;
    _oled_col_count  = oled_columns;

    int i = 0;
    _oled_data_buf[i++] = SSD1306_COMM_CONTROL_BYTE;  //command control byte
    _oled_data_buf[i++] = SSD1306_COMM_DISPLAY_OFF;   //display off
    _oled_data_buf[i++] = SSD1306_COMM_DISP_NORM;     //Set Normal Display (default)
    _oled_data_buf[i++] = SSD1306_COMM_CLK_SET;       //SETDISPLAYCLOCKDIV
    _oled_data_buf[i++] = 0x80;                       // the suggested ratio 0x80
    _oled_data_buf[i++] = SSD1306_COMM_MULTIPLEX;     //SSD1306_SETMULTIPLEX
    _oled_data_buf[i++] = oled_lines - 1;             // height is 32 or 64 (always -1)
    _oled_data_buf[i++] = SSD1306_COMM_VERT_OFFSET;   //SETDISPLAYOFFSET
    _oled_data_buf[i++] = 0;                          //no offset
    _oled_data_buf[i++] = SSD1306_COMM_START_LINE;    //SETSTARTLINE
    _oled_data_buf[i++] = SSD1306_COMM_CHARGE_PUMP;   //CHARGEPUMP
    _oled_data_buf[i++] = 0x14;                       //turn on charge pump
    _oled_data_buf[i++] = SSD1306_COMM_MEMORY_MODE;   //MEMORYMODE
    _oled_data_buf[i++] = SSD1306_HORI_MODE;          // page mode
    _oled_data_buf[i++] = (rotate ? SSD1306_COMM_HORIZ_FLIP : SSD1306_COMM_HORIZ_NORM);	//SEGREMAP
    _oled_data_buf[i++] = (rotate ? SSD1306_COMM_SCAN_REVS : SSD1306_COMM_SCAN_NORM);		//COMSCANDEC
    _oled_data_buf[i++] = SSD1306_COMM_COM_PIN;       //HARDWARE PIN 
    if (oled_lines == 32)
        _oled_data_buf[i++] = 0x02;                       // for 32 lines
    else
        _oled_data_buf[i++] = 0x12;                       // for 64 lines or 48 lines
    _oled_data_buf[i++] = SSD1306_COMM_CONTRAST;      //SETCONTRAST
    _oled_data_buf[i++] = 0x7f;                       // default contract value
    _oled_data_buf[i++] = SSD1306_COMM_PRECHARGE;     //SETPRECHARGE
    _oled_data_buf[i++] = 0xf1;                       // default precharge value
    _oled_data_buf[i++] = SSD1306_COMM_DESELECT_LV;   //SETVCOMDETECT                
    _oled_data_buf[i++] = 0x40;                       // default deselect value
    _oled_data_buf[i++] = SSD1306_COMM_RESUME_RAM;    //DISPLAYALLON_RESUME
    _oled_data_buf[i++] = SSD1306_COMM_DISP_NORM;     //NORMALDISPLAY
    _oled_data_buf[i++] = SSD1306_COMM_DISPLAY_ON;    //DISPLAY ON             
    _oled_data_buf[i++] = SSD1306_COMM_DISABLE_SCROLL;//Stop scroll
    
    return i2c_write_bytes(fd, SSD1306_I2C_ADDR, _oled_data_buf, i);
}

int oled_set_xy(int fd, int x, int y)
{
    _oled_data_buf[0] = SSD1306_COMM_CONTROL_BYTE;
    _oled_data_buf[1] = SSD1306_COMM_PAGE_NUMBER | (y & 0x0f);
    _oled_data_buf[2] = SSD1306_COMM_LOW_COLUMN | (x & 0x0f);   
    _oled_data_buf[3] = SSD1306_COMM_HIGH_COLUMN | ((x >> 4) & 0x0f);
    return i2c_write_bytes(fd, SSD1306_I2C_ADDR, _oled_data_buf, 4);
}

int oled_clear(int fd)
{
	memset(&_oled_data_buf, 0, sizeof(_oled_data_buf));
	_oled_data_buf[0] = SSD1306_DATA_CONTROL_BYTE;
	int r = i2c_write_bytes(fd, SSD1306_I2C_ADDR, _oled_data_buf, 1 + (_oled_line_count * _oled_col_count / 8));
	if (r < 0) return r;
	return oled_set_xy(fd, 0, 0);
}

int oled_show_bitmap(int fd, const unsigned char *data, int dataLen)
{
	_oled_data_buf[0] = SSD1306_DATA_CONTROL_BYTE;
	memcpy(&_oled_data_buf[1], data, dataLen);
	return i2c_write_bytes(fd, SSD1306_I2C_ADDR, _oled_data_buf, 1 + dataLen);
}
