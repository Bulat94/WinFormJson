# Демо УЦ - модуль сертификации.


## Описание демо стенда модуля сертификации

Модуль сертификации представляет собой упрощенную реализацию инфраструктуры открытых ключей (PKI) только для демонстрационных целей. Важно отметить, что модуль не является удостоверяющим центром, но выполняет часть его функций.

В системе реализована следующая функциональность:
- выпуск корневого сертификата.
- выпуск промежуточных сертификатов.
- выпуск сертификатов пользователей по запросу на сертификат.

Вышеперечисленные методы доступны по api. Для просмотра api добавлен swagger, описание методов доступно на главной странице.


## Стек технологий

| <!-- -->         | <!-- -->              |
| ---              | ---                   |
| Веб-сервер:      | .NET Core 8.          |   
| База данных:     | PostgreSQL            |
| Библиотеки:      | [Bouncy Castle Cryptography Library For .NET](https://github.com/bcgit/bc-csharp).   |

Сервис работает с сертификатами в формате X.509,<br />
на данный момент реализована работа только с алгоритмом ГОСТ Р 34.10-2012 256-бит.<br />


## Конфигурация сервиса

В качестве хранилища данных сервис использует СУБД PostgreSQL,<br />
ConnectionString к базе необходимо установить в файлах `AktivCA.Web/appsettings.json`, `AktivCA.Domain.DbMigrator/appsettings.json`.

В файле `AktivCA.Web/appsettings.json` в разделе CertificateParams имеются параметры для конфигурации сервиса

| <!-- -->                      | <!-- -->                                                                              |
| ---                           | ---                                                                                   |
| IsRootCa:                     | признак того, является ли модуль корневым                                             | 
| Name:                         | Наименование модуля, указывается в корневом сертификате (issuer DN)                   |   
| RootCertDurationInYears:      | срок валидности самоподписанного корневого сертификата, актуально для корневого модуля|
| IntermediateDurationInYears:  | срок валидности генерируемого сертификата по запросу для промежуточных модулей        |
| CaUrl:                        | url вышестоящего модуля, актуально для промежуточного модуля                          |
| UserCertDurationInYears:      | срок валидности пользовательских сертификатов                                         |


### Сборка 

Для сборки и запуска веб-сервиса необходим установленный в системе [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).<br />
Сборка запускается командой dotnet publish -c Release


## Установка зависимостей и запуск сервиса локально

Для миграций в бд предусмотрен отдeльный проект, для запуска миграций необходимо выполнить команды 

- cd AktivCA.Domain.DbMigrator/bin/Release/net8.0/publish/
- dotnet AktivCA.Domain.DbMigrator.dll

Для запуска сервиса, необходимо в директории проекта выполнить команды:

- cd AktivCA.Web\bin\Release\net8.0
- dotnet AktivCA.Web.dll


## Установка зависимостей и запуск сервиса в docker

Для запуска в докер контейнере необходимо сконфигурировать .env файл в корневой директории проекта, параметры:


| <!-- -->                      | <!-- -->                     |
| ---                           | ---                          |
| APP_CONTAINER_NAME:           | Имя контейнера приложения    |
| DESTINATION_FOLDER:           | Директория контейнера        |
| APP_PORT:                     | Внешний порт приложения      |
| DB_CONTAINER_NAME:            | Имя контейнера бд            |
| DB_NAME:                      | Наименование бд              |
| DB_PORT:                      | Внешний порт бд              |
| DB_PSWD:                      | Пароль пользователя бд       |
| DB_USER:                      | Имя пользователя бд          |

Создать директорию контейнера, например sudo mkdir /Users/my-user/docker/ca-demo

инициализировать директории проектов
```
destinationFolder='/Users/my-user/docker/ca-demo'
sudo mkdir -p ${destinationFolder}/var/app
sudo mkdir -p ${destinationFolder}/var/migrator
sudo mkdir -p ${destinationFolder}/var/lib/postgresql/data
```

перейти в директорию проекта (укажите вашу директорию)<br />
```сd myproject/path```

скопировать сборки и файлы docker
```
scp docker-compose.yml ${destinationFolder}
scp .env ${destinationFolder}
scp -r AktivCA.Web/bin/Release/net8.0/publish/* ${destinationFolder}var/app
scp -r AktivCA.Domain.DbMigrator/bin/Release/net8.0/publish/* ${destinationFolder}var/migrator
```

запустить контейнеры ```sudo docker compose -f ${destinationFolder}docker-compose.yml up```
