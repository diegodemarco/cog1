systemctl stop cog1

cd Native/cog1_io
make
cd ../..

cd Native/cog1_logo
make
cd ../..

cp Native/cog1_logo/cog1_logo /cog1/
cp Native/cog1_io/cog1_io.so /cog1/
cp ./cog1app /cog1/

rm -r /cog1/admin
mkdir /cog1/admin
cp -r admin/* /cog1/admin

systemctl start cog1