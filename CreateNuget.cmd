if not exist .\nuget_packages mkdir nuget_packages
if not exist .\distro mkdir distro
xcopy EFCoreEagerFetching\*.cs distro\src\ /Y /Q /E
xcopy EFCoreEagerFetching\bin\Debug\netcoreapp2.0\*.*  distro\lib\core20\ /Q
xcopy EFCoreEagerFetching.nuspec           distro\ /Q
cd distro

..\nuget.exe pack EFCoreEagerFetching.nuspec -symbols -Prop Platform=AnyCPU

xcopy *.nupkg ..\nuget_packages\ /Y /Q

pause
cd ..
rmdir distro  /s /q
