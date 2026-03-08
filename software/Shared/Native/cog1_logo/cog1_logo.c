#include <stdio.h>
#include <time.h>
#include <errno.h> 
#include <fcntl.h>
#include <unistd.h>
#include <sys/ioctl.h>
#include "../include/cog1_oled.h"
#include "../include/cog1_i2c.h"
#include "logo.h"

int i2c_fd = 0;
const char *i2c_device = "/dev/i2c-3";

int main()
{
	int i2c_fd = i2c_open(i2c_device);
    if (i2c_fd < 0)
    {
		printf("Error opening I2C device: %d\n", i2c_fd);
		return -1;
    }
	
	int r = oled_config(i2c_fd, 64, 128, 1);
	if (r > 0)
	{
		r = oled_set_xy(i2c_fd, 0, 0);
		if (r > 0)
		{
			r = oled_show_bitmap(i2c_fd, oled_logo, sizeof(oled_logo));
			if (r <= 0) 
			{
				printf("oled_show_bitmap error: %d\n", r);
			}
		}
		else 
		{
			printf("oled_clear error: %d\n", r);
		}
	}
	else
	{
		printf("oled_default_config error: %d\n", r);
	}
	
	return 0;
}
