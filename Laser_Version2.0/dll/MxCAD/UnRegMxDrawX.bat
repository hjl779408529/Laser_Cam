Echo off
Echo._______________________________________________________________
Echo.
Echo.   ����ע���ؼ�(��ע��ʧ�����Թ���Ա�������)....
Echo.
Echo.   http://www.mxdraw.com
Echo.________________________________________________________________

if /i "%PROCESSOR_IDENTIFIER:~0,3%" == "X86" goto X32

cd 64bit
%systemroot%\system32\regsvr32 MxDrawX.ocx /u

exit

:X32
cd 32bit
regsvr32 MxDrawX.ocx /u

exit