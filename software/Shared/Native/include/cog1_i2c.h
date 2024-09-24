#ifndef __COG1_I2C_H__
#define __COG1_I2C_H__

extern int i2c_open(const char *dev);
extern int i2c_read_bytes(int fd, unsigned char deviceAddress, unsigned char *data, unsigned int dataLen);
extern int i2c_write_byte(int fd, unsigned char deviceAddress, unsigned char data);
extern int i2c_write_bytes(int fd, unsigned char deviceAddress, unsigned char *data, unsigned int dataLen);
extern int i2c_read_bytes_register(int fd, unsigned char deviceAddress, unsigned char registerAddress, unsigned char *data, unsigned int dataLen);
extern int i2c_write_byte_register(int fd, unsigned char deviceAddress, unsigned char registerAddress, unsigned char data);
extern int i2c_write_bytes_register(int fd, unsigned char deviceAddress, unsigned char registerAddress, unsigned char *data, unsigned int dataLen);

#endif
