## Run
dotnet run --project RxCats.RealTime

## Test Json

### Connect (1)

```
{"MessageType":"Connect","Message":{"CharacterNo":1,"Nickname":"1"}}
```

### Connect (2)
```
{"MessageType":"Connect","Message":{"CharacterNo":2,"Nickname":"2"}}
```

### ChatMessage (1)
```
{"MessageType":"GameChat","Message":{"CharacterNo":1,"GameNo":0,"Message":"Hello World!"}}
```