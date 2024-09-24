#!/bin/bash
cp ./sun50i-h616-pcf-8523.dtso /boot/dtb/allwinner/overlay
pushd /boot/dtb/allwinner/overlay
dtc --in-format dts sun50i-h616-pcf-8523.dtso --out sun50i-h616-pcf-8523.dtbo
popd
