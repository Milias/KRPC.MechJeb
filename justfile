# KRPC.MechJeb — local build wrapper for the mechjeb-2.15-port branch.
# Mirrors the .csproj compile list. `lib/` is populated by `just fetch-libs`
# (rsync'd from archlinux-home's KSP install). DLL drops into the host's
# GameData/kRPC/ via `just install`.

ksp_host    := env_var_or_default("KSP_HOST", "archlinux-home")
ksp_path    := env_var_or_default("KSP_PATH", "/home/francisco/.local/share/Steam/steamapps/common/Kerbal Space Program")

managed_libs := "Assembly-CSharp.dll UnityEngine.dll UnityEngine.CoreModule.dll"
krpc_libs    := "KRPC.dll KRPC.Core.dll KRPC.SpaceCenter.dll"

default: build

# One-time: rsync KSP + kRPC DLLs into lib/.
fetch-libs:
    mkdir -p lib
    for f in {{managed_libs}}; do \
        rsync -av "{{ksp_host}}:{{ksp_path}}/KSP_x64_Data/Managed/$f" lib/; \
    done
    for f in {{krpc_libs}}; do \
        rsync -av "{{ksp_host}}:{{ksp_path}}/GameData/kRPC/$f" lib/; \
    done
    ls -l lib/

# Compile every .cs (mirroring the .csproj include list — no test files
# present, so a recursive glob is equivalent).
build:
    mkdir -p bin
    mcs -target:library -out:bin/KRPC.MechJeb.dll \
        -recurse:'*.cs' \
        -r:lib/Assembly-CSharp.dll \
        -r:lib/UnityEngine.dll \
        -r:lib/UnityEngine.CoreModule.dll \
        -r:lib/KRPC.dll \
        -r:lib/KRPC.Core.dll \
        -r:lib/KRPC.SpaceCenter.dll
    ls -l bin/KRPC.MechJeb.dll

install: build
    scp bin/KRPC.MechJeb.dll "{{ksp_host}}:{{ksp_path}}/GameData/kRPC/"
    ssh "{{ksp_host}}" 'ls -l "{{ksp_path}}/GameData/kRPC/KRPC.MechJeb.dll"'

clean:
    rm -rf bin
