# FileExplorer

## Развертывание
- Иметь SqlServer 15.x
- Иметь .NET SDK 7.x и ASP.NET Core Runtime 7.x
- Скачать или склонировать репозиторий
- Запустить проект

## API
Протестировать API можно через Swagger, или импортировать файл FileExplorerAPI.postman_collection.json в Postman

- /files/get - получить информацию о всех файлах
- /files/get/{id} - получить информацию об указанном файле
- /files/download?ids - скачать указанные файлы (ответ в формате .zip)
- /files/download/{id} - скачать указанный файл
- /files/upload - загрузить файл или группу файлов
- /files/share/{guid} - скачать файлы по одноразовой ссылке (ответ в формате .zip, доступно неавторизованным пользователям, можно скачать чужие файлы)
- /files/progress/{guid} - получить статус загрузки файла или группы файлов

- /login - зарегистрироваться или войти в аккаунт
- /logout - выйти из аккаунта

- /link/create/onetime?ids - создать одноразовую ссылку на скачивание указанных файлов
