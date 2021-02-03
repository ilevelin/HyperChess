@echo off
echo "Are you sure you want to delete all your logs? Doing it with this script can't be undone."
echo "[ACCEPT] > Press any key."
echo "[CANCEL] > Close this window or press ctrl+c."
pause
for %%i in (LOG_????_??_??_??_??_??.txt) do (
	echo "DELETING > %%i"
	del %%i
)
if exist LOG_????_??_??_??_??_??.txt.meta (
	echo "INFO     > DETECTED UNITY META FILES, DELETING..."
	for %%i in (LOG_????_??_??_??_??_??.txt.meta) do (
		echo "DELETING > %%i"
		del %%i
	)
)
echo "All logs deleted, press any key to exit."
pause