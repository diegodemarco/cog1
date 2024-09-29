clear

systemctl stop cog1

cd Native/cog1_io
make
cd ../..

cd Native/cog1_logo
make
cd ../..

cp Native/cog1_logo/cog1_logo .
cp Native/cog1_io/cog1_io.so .

./cog1