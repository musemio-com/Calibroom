# MECM Calibroom (Calibration - Room)
MECM plugin dev for XR4ALL

# Installation
## Install InteractML as a git submodule
```
# initiate git in folder (if not done already by the Github Desktop GUI)
git clone URL repo

# go to repo's folder
cd nameOfFolder

# add InteractML as a submodule (currently using the VRInterface branch)
git submodule add -b VRInterface --force https://github.com/Interactml/iml-unity.git Assets/iml-unity

# go to submodule folder
cd Assets/iml-unity/
# set the iml submodule to only use the Assets folder
git sparse-checkout init --cone
git sparse-checkout set Assets
# leave submodule folder
cd ../..
```

## Add the following packages in Unity's Package Manager Window
1. Open the project in Unity 2020.1
2. Go to Window > Package Manager
3. Install Input System
~~Install XR Interaction Toolkit (requires showing preview packages set to true)~~ We include a modified version of XR Interaction Toolkit v0.9.4 as part of the source code now, no need for this step

## Add Firebase packages in Unity's Package Manager Window
1. Download the required firebase Unity **.tgz** packages from here (currently using v7.1.0): https://developers.google.com/unity/archive 
2. Install the following packages using the Package Manager window in the **correct order** ([see here to install .tgz packages](https://docs.unity.cn/2020.1/Documentation/Manual/upm-ui-tarball.html) ):

⋅⋅⋅1. External Dependency Manager [`com.google.external-dependency-manager`](https://dl.google.com/games/registry/unity/com.google.external-dependency-manager/com.google.external-dependency-manager-1.2.164.tgz) ⋅⋅
⋅⋅⋅2. Firebase Core [`com.google.firebase.app`](https://dl.google.com/games/registry/unity/com.google.firebase.app/com.google.firebase.app-7.1.0.tgz) ⋅⋅
⋅⋅⋅3. Firebase Auth [`com.google.firebase.auth`](https://dl.google.com/games/registry/unity/com.google.firebase.auth/com.google.firebase.auth-7.1.0.tgz) ⋅⋅
⋅⋅⋅4. Firebase Cloud Storage [`com.google.firebase.storage`](https://dl.google.com/games/registry/unity/com.google.firebase.storage/com.google.firebase.storage-7.1.0.tgz) ⋅⋅