cog1_io.so: cog1_io.o cog1_i2c.o cog1_oled.o
	gcc -shared -o cog1_io.so cog1_io.o cog1_i2c.o cog1_oled.o -lgpiod

cog1_io.o: cog1_io.c
	gcc -c -Wall -Werror -fpic cog1_io.c

cog1_i2c.o:
	gcc -c -Wall -Werror -fpic cog1_i2c.c

cog1_oled.o: ../include/cog1_oled.h
	gcc -c -Wall -Werror -fpic cog1_oled.c