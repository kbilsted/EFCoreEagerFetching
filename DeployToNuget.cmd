REM ON FIRST RUN, RUN THIS (change the key to whatever is found on your profile on www.nuget.org ->   
REM .\NuGet.exe setapikey a2d2d22a-2322-2a... whatever you have on your profile at nuget.org

call CreateNuget.cmd

.\NuGet.exe push nuget_packages\*.symbols.nupkg -Source https://nuget.smbsrc.net/
.\NuGet.exe push nuget_packages\*.symbols.nupkg -Source https://www.nuget.org/api/v2/package
.\NuGet.exe push nuget_packages\*.nupkg -Source https://www.nuget.org/api/v2/package

cd nuget_packages
del /q *
cd ..
REM rmdir nuget_packages

pause 