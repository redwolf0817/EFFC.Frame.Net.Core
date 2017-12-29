@ echo off
:: 设置压缩JS文件的根目录，脚本会自动按树层次查找和压缩所有的JS
SET JSFOLDER=%cd%
echo 正在查找JS文件
chdir /d %JSFOLDER%
for /r . %%a in (*.min.js) do (
    @echo 正在删除 %%~pa%%~na.min.js ...
    del %%~fa

)
for /r . %%a in (*.js) do (
    @echo 正在压缩 %%~pa%%~na.min.js ...
    uglifyjs %%~fa  -m -o %%~pa%%~na.min.js

)
echo 完成!
::pause & exit