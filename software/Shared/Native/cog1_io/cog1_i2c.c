#include <stdio.h>
#include <fcntl.h>
#include <errno.h>
#include <string.h>
#include <unistd.h>
#include <sys/ioctl.h>
#include <linux/i2c-dev.h>

int _i2c_set_address(int fd, unsigned char address)
{
	return ioctl(fd, I2C_SLAVE, (char *)(long)address);
}

int i2c_open(const char *dev)
{
	return open(dev, O_RDWR);
}

int i2c_read_bytes(int fd, unsigned char deviceAddress, unsigned char *data, unsigned int dataLen)
{
	int r = _i2c_set_address(fd, deviceAddress);
	if (r < 0) return r;
	return read(fd, (void *)data, dataLen);
}

int i2c_write_byte(int fd, unsigned char deviceAddress, unsigned char data)
{
	int r = _i2c_set_address(fd, deviceAddress);
	if (r < 0) return r;
	return write(fd, &data, 1);
}

int i2c_write_bytes(int fd, unsigned char deviceAddress, unsigned char *data, unsigned int dataLen)
{
    int r = _i2c_set_address(fd, deviceAddress);
	if (r < 0) return r;
	return write(fd, data, dataLen);
}

int i2c_read_bytes_register(int fd, unsigned char deviceAddress, unsigned char registerAddress, unsigned char *data, unsigned int dataLen)
{
	int r = _i2c_set_address(fd, deviceAddress);
	if (r < 0) return r;
	r = write(fd, &registerAddress, 1);
	if (r < 0) 
	{
		printf("i2c write error%d: %s\n", r, strerror(errno));
		return r;
	}
	r = read(fd, (void *)data, dataLen);
	if (r < 0) 
	{
		printf("i2c read error %d: %s\n", r, strerror(errno));
		return r;
	}
	return r;
}

int i2c_write_bytes_register(int fd, unsigned char deviceAddress, unsigned char registerAddress, unsigned char *data, unsigned int dataLen)
{
    unsigned char i;
    unsigned char buffer[200];

    buffer[0] = registerAddress;
    for (i = 1; i <= dataLen; i++)
        buffer[i] = data[i - 1];
    int r = _i2c_set_address(fd, deviceAddress);
	if (r < 0) return r;
	r = write(fd, buffer, dataLen + 1);
	if (r < 0) return r;
	return (r - 1);		// Return number of bytes written, without counting the register address
}

int i2c_write_byte_register(int fd, unsigned char deviceAddress, unsigned char registerAddress, unsigned char data)
{
    return i2c_write_bytes_register(fd, deviceAddress, registerAddress, &data, 1);
}
