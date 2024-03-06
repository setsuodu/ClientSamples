# Samples

## TODO:
- [x] 1. 抽离ABManager相关的代码，做成Package包。
	- 可随时切换AA/AB，不影响项目？
		- LoadPrefab()等API保留。
- [ ] 2. 做一个Editor，要求填写ProductName，CompanyName，等，填完提交，自动生成配置/常量脚本。起到提示作用。
- [ ] 3. 项目里自建一个Web，StreamingAssets中。
- [ ] 4. 配打包脚本。

## 注意
Q: assetbundle资源重复打包问题，目前的结构，是否存在两个预设（prefab）都引用一张图，该贴图打包两份问题？
A: 感觉不存在重复，以前解决过。
   重复引用的资源，打包成一个预设，shared.unity3d


最容易重复资源：模型引用材质贴图，UI引用贴图。
跑脚本，分析依赖关系。


资源卸载，内存
https://blog.csdn.net/u013774978/article/details/129847761

Memory Profiler
https://blog.csdn.net/mango9126/article/details/130752576