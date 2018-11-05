Echo off
Echo._______________________________________________________________
Echo.
Echo.   正在注册控件(如注册失败请以管理员身份运行)....
Echo.
Echo.   http://www.mxdraw.com
Echo.________________________________________________________________

if /i "%PROCESSOR_IDENTIFIER:~0,3%" == "X86" goto X32


Echo.注册64位控件
cd 64bit
%systemroot%\system32\regsvr32 MxDrawX.ocx


Echo.注册32位控件
cd ..
cd 32bit
%systemroot%\syswow64\regsvr32 MxDrawX.ocx

exit

:X32
cd 32bit
regsvr32 MxDrawX.ocx

exit