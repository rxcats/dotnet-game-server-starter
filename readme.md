## Run
```
dotnet run --project RxCats.RealTime
```

## Test Json

### Connect (1)

```
{"MessageType":"Connect","Message":{"CharacterNo":1,"Nickname":"C1"}}
```

### CreateGame
```
{"MessageType":"CreateGame","Message":{"GameName":"C1Game"}}
```

### LeaveGame - Requre Fix GameNo
```
{"MessageType":"LeaveGame","Message":{"GameNo":1}}
```

### ChatMessage - Requre Fix GameNo
```
{"MessageType":"GameChat","Message":{"GameNo":1,"Message":"Hello World!"}}
```


### Connect (2)
```
{"MessageType":"Connect","Message":{"CharacterNo":2,"Nickname":"C2"}}
```

### JoinGame - Requre Fix GameNo
```
{"MessageType":"JoinGame","Message":{"GameNo":1}}
```

### LeaveGame - Requre Fix GameNo
```
{"MessageType":"LeaveGame","Message":{"GameNo":1}}
```

### ChatMessage - Requre Fix GameNo
```
{"MessageType":"GameChat","Message":{"GameNo":1,"Message":"Hello World!"}}
```