# Модуль сертификации

## Описание демо стенда модуля сертификации

Модуль сертификации представляет собой упрощенную реализацию инфраструктуры открытых ключей (PKI) только для демонстрационных целей. Важно отметить, что модуль не является удостоверяющим центром, но выполняет часть его функций.

В системе реализована следующая функциональность:
- выпуск корневого сертификата;
- выпуск промежуточных сертификатов;
- выпуск сертификатов пользователей по запросу на сертификат.

Вышеперечисленные методы доступны по API. Для просмотра API добавлен swagger, описание методов доступно на главной странице.

## Стек технологий

| <!-- -->         | <!-- -->                                                                          |
| ---              | ---                                                                               |
| Веб-сервер       | [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)              |   
| База данных      | PostgreSQL                                                                        |
| Библиотеки       | [Bouncy Castle Cryptography Library For .NET](https://github.com/bcgit/bc-csharp) |

Сервис работает с сертификатами в формате X.509.
> На данный момент реализована работа только с алгоритмом ГОСТ Р 34.10-2012 256-бит.

## Конфигурация сервиса

В качестве хранилища данных сервис использует СУБД PostgreSQL.

`ConnectionString` к базе необходимо указать в файлах:
- `AktivCA.Web/appsettings.json`;
- `AktivCA.Domain.DbMigrator/appsettings.json`.

В файле `AktivCA.Web/appsettings.json` в разделе `CertificateParams` имеются параметры для конфигурации сервиса.

| Параметр                    | Описание                                                                               |
| ---                         | ---                                                                                    |
| IsRootCa                    | Признак того, является ли модуль корневым.                                             | 
| Name                        | Наименование модуля, указывается в корневом сертификате (issuer DN).                   |   
| RootCertDurationInYears     | Срок валидности самоподписанного корневого сертификата, актуально для корневого модуля.|
| IntermediateDurationInYears | Срок валидности генерируемого сертификата по запросу для промежуточных модулей.        |
| CaUrl                       | URL вышестоящего модуля, актуально для промежуточного модуля.                          |
| UserCertDurationInYears     | Срок валидности пользовательских сертификатов.                                         |

## Сборка 

Для сборки и запуска сервиса необходим установленный в системе [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). Запустить сервис можно как локально, так и в Docker.

Сборка запускается следующей командой — `dotnet publish -c Release`.

### Установка зависимостей и запуск сервиса локально

Для миграций в БД предусмотрен отдeльный проект, для запуска миграций необходимо выполнить следующие команды.

> __Linux__

- `cd AktivCA.Domain.DbMigrator/bin/Release/net8.0/publish/`
- `dotnet AktivCA.Domain.DbMigrator.dll`

> __Windows__

- `cd AktivCA.Domain.DbMigrator\bin\Release\net8.0\publish\`
- `dotnet AktivCA.Domain.DbMigrator.dll`

Для запуска сервиса, необходимо в директории проекта выполнить следующие команды.

> __Linux__

- `cd AktivCA.Web/bin/Release/net8.0`
- `dotnet AktivCA.Web.dll`

> __Windows__

- `cd AktivCA.Web\bin\Release\net8.0`
- `dotnet AktivCA.Web.dll`

Перейдите по адресу http://localhost:5000/ (адрес также указан в консоли после запуска сервиса) для просмотра swagger, описание методов доступно на главной странице. 

### Установка зависимостей и запуск сервиса в Docker

1. Для запуска в докер контейнере необходимо сконфигурировать `.env` файл в корневой директории проекта. Описание параметров представлено ниже в таблице.


    | Параметр                      | Описание                     |
    | ---                           | ---                          |
    | APP_CONTAINER_NAME            | Имя контейнера приложения    |
    | DESTINATION_FOLDER            | Директория контейнера        |
    | APP_PORT                      | Внешний порт приложения      |
    | DB_CONTAINER_NAME             | Имя контейнера БД            |
    | DB_NAME                       | Наименование БД              |
    | DB_PORT                       | Внешний порт БД              |
    | DB_PSWD                       | Пароль пользователя БД       |
    | DB_USER                       | Имя пользователя БД          |

    > Параметры `ConnectionString` в файлах `AktivCA.Web/appsettings.json` и `AktivCA.Domain.DbMigrator/appsettings.json` должны совпадать с параметрами, указанными в файле `.env`.
    > ```json
    > {
    >     "ConnectionStrings": {
    >         "Default": "Host=DB_CONTAINER_NAME;Port=DB_PORT;Database=DB_NAME;Username=DB_USER;Password=DB_PSWD"
    >     },
    > }
    > ```

2. Создать директорию контейнера.

    Пример — `mkdir /Users/my-user/ca-demo`.

3. Инициализировать директории проектов.

    ```sh
    destinationFolder='/Users/my-user/ca-demo'
    
    mkdir -p ${destinationFolder}/var/app
    mkdir -p ${destinationFolder}/var/migrator
    mkdir -p ${destinationFolder}/var/lib/postgresql/data
    ```

4. Перейти в директорию проекта "Модуль сертификации" (укажите вашу директорию).

    ```sh
    сd /Users/my-user/myproject/
    ```

5. Скопировать сборки и файлы docker.

    ```sh
    scp docker-compose.yml ${destinationFolder}
    scp .env ${destinationFolder}
    scp -r AktivCA.Web/bin/Release/net8.0/publish/* ${destinationFolder}/var/app
    scp -r AktivCA.Domain.DbMigrator/bin/Release/net8.0/publish/* ${destinationFolder}/var/migrator
    ```

6. Запустить контейнеры.

    ```sh
    docker compose -f ${destinationFolder}/docker-compose.yml up
    ```

7. После запуска контейнера перейти `http://localhost:APP_PORT`, где **APP_PORT** — параметр заданный в файле `.env` на первом шаге.
