# SafeScribe API

Projeto de API RESTful para gestão de notas com autenticação JWT e controle de acesso por papéis (Roles).

## Integrantes

* RM554992 - Rafael Macoto Magalhães Seo - 2TDSPZ
* RM556270 - Bianca Vitoria - 2TDSPZ
* RM558976 - Maria Eduarda Pires Vieira - 2TDSPZ

## Funcionalidades

* Autenticação JWT (Registro e Login).
* Logout efetivo com Blacklist de Token.
* Autorização por Papéis (Admin, Editor, Reader).
* Hashing de Senhas com BCrypt.
* CRUD de Anotações com regras de permissão.
* Documentação com Swagger.

## Tecnologias

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core In-Memory
* Autenticação JWT (JwtBearer)
* BCrypt.Net

## Como Executar

1.  Clone o repositório.
2.  Execute o projeto (F5 no Visual Studio ou `dotnet run`).
3.  A API estará disponível em `http://localhost:58696`.
4.  A documentação do Swagger estará em `http://localhost:58696/swagger`.

### Usuários Padrão

O banco de dados em memória é iniciado com estes usuários para teste:

| Usuário | Senha | Papel |
| :--- | :--- | :--- |
| `admin` | `admin123` | Admin |
| `editor` | `editor123` | Editor |
| `reader` | `reader123` | Reader |

## Endpoints da API

### Autenticação (`/api/v1/auth`)

* `POST /registrar` (Público)
* `POST /login` (Público)
* `POST /logout` (Autenticado)

### Notas (`/api/v1/notas`)

* `POST /` (Acesso: `Admin`, `Editor`)
* `GET /{id}` (Acesso: `Admin`, `Editor`, `Reader`)
    * *Regra: Editor/Reader só podem ver suas próprias notas.*
* `PUT /{id}` (Acesso: `Admin`, `Editor`)
    * *Regra: Editor só pode atualizar suas próprias notas.*
* `DELETE /{id}` (Acesso: `Admin`)
