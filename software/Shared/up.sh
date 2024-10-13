systemctl stop cog1

cd Native/cog1_io
make
cd ../..

cd Native/cog1_logo
make
cd ../..

cp Native/cog1_logo/cog1_logo /cog1/
cp Native/cog1_io/cog1_io.so /cog1/
cp ./libe_sqlite3.so /cog1/
cp ./cog1 /cog1/

rm -rf /cog1/admin
rm -rf /cog1/console
mkdir /cog1/console
cp -r console/* /cog1/console

systemctl start cog1