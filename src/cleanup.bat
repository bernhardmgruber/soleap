@echo off
for /d %%p in (SoLeap.*) do ^
for %%d in (bin,obj) do ^
if exist %%p\%%d ^
rd /s /q %%p\%%d & echo Removed %%p\%%d