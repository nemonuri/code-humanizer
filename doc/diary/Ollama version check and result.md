# Ollama version check and result

## When local ollama server is not running

```
PS > ollama --version
Warning: could not connect to a running Ollama instance
Warning: client version is 0.9.0
```

## When local ollama server is running

```
PS > ollama --version
ollama version is 0.9.0
```

## When local ollama host command is wrong

```
PS > ollama2 --version
ollama2: The term 'ollama2' is not recognized as a name of a cmdlet, function, script file, or executable program.
Check the spelling of the name, or if a path was included, verify that the path is correct and try again.
```