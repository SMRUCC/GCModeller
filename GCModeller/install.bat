cd bin

for %%i in (*.exe) do (
    %%i /linux-bash
)

cd ..