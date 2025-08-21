# Ollama serve log

```
time=2025-08-21T14:19:24.688+09:00 level=INFO source=routes.go:1234 msg="server config" env="map[CUDA_VISIBLE_DEVICES: GPU_DEVICE_ORDINAL: HIP_VISIBLE_DEVICES: HSA_OVERRIDE_GFX_VERSION: HTTPS_PROXY: HTTP_PROXY: NO_PROXY: OLLAMA_CONTEXT_LENGTH:4096 OLLAMA_DEBUG:INFO OLLAMA_FLASH_ATTENTION:false OLLAMA_GPU_OVERHEAD:0 OLLAMA_HOST:http://127.0.0.1:11434 OLLAMA_INTEL_GPU:false OLLAMA_KEEP_ALIVE:5m0s OLLAMA_KV_CACHE_TYPE: OLLAMA_LLM_LIBRARY: OLLAMA_LOAD_TIMEOUT:5m0s OLLAMA_MAX_LOADED_MODELS:0 OLLAMA_MAX_QUEUE:512 OLLAMA_MODELS:(...) OLLAMA_MULTIUSER_CACHE:false OLLAMA_NEW_ENGINE:false OLLAMA_NOHISTORY:false OLLAMA_NOPRUNE:false OLLAMA_NUM_PARALLEL:0 OLLAMA_ORIGINS:[http://localhost https://localhost http://localhost:* https://localhost:* http://127.0.0.1 https://127.0.0.1 http://127.0.0.1:* https://127.0.0.1:* http://0.0.0.0 https://0.0.0.0 http://0.0.0.0:* https://0.0.0.0:* app://* file://* tauri://* vscode-webview://* vscode-file://*] OLLAMA_SCHED_SPREAD:false ROCR_VISIBLE_DEVICES:]"
time=2025-08-21T14:19:24.700+09:00 level=INFO source=images.go:479 msg="total blobs: 42"
time=2025-08-21T14:19:24.700+09:00 level=INFO source=images.go:486 msg="total unused blobs removed: 0"
time=2025-08-21T14:19:24.701+09:00 level=INFO source=routes.go:1287 msg="Listening on 127.0.0.1:11434 (version 0.9.0)"
time=2025-08-21T14:19:24.702+09:00 level=INFO source=gpu.go:217 msg="looking for compatible GPUs"
time=2025-08-21T14:19:24.702+09:00 level=INFO source=gpu_windows.go:167 msg=packages count=1
time=2025-08-21T14:19:24.702+09:00 level=INFO source=gpu_windows.go:183 msg="efficiency cores detected" maxEfficiencyClass=1
time=2025-08-21T14:19:24.702+09:00 level=INFO source=gpu_windows.go:214 msg="" package=0 cores=16 efficiency=8 threads=24
time=2025-08-21T14:19:24.849+09:00 level=INFO source=gpu.go:319 msg="detected OS VRAM overhead" id=(...) library=cuda compute=8.6 driver=12.6 name="NVIDIA GeForce RTX 3060 Ti" overhead="582.4 MiB"
time=2025-08-21T14:19:24.850+09:00 level=INFO source=types.go:130 msg="inference compute" id=(...) library=cuda variant=v12 compute=8.6 driver=12.6 name="NVIDIA GeForce RTX 3060 Ti" total="8.0 GiB" available="7.0 GiB"
```