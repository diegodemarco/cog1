#ifndef __COG1_OLED_H__
#define __COG1_OLED_H__

extern int oled_config(int fd, int oled_lines, int oled_columns, int rotate);
extern int oled_set_xy(int fd, int x, int y);
extern int oled_clear(int fd);
extern int oled_show_bitmap(int fd, const unsigned char *data, int dataLen);

#endif
