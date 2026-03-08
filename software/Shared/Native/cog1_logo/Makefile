cog1_logo: cog1_logo.o cog1_i2c.o cog1_oled.o
	gcc -Wall -g -o cog1_logo cog1_logo.o cog1_i2c.o cog1_oled.o
	
cog1_logo.o: cog1_logo.c logo.h
	gcc -c -Wall -Werror -fpic cog1_logo.c
	
cog1_i2c.o:
	gcc -c -Wall -Werror -fpic ../cog1_io/cog1_i2c.c

cog1_oled.o: ../cog1_io/cog1_oled.c ../include/cog1_oled.h
	gcc -c -Wall -Werror -fpic ../cog1_io/cog1_oled.c