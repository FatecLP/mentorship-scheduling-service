# Mentorship Scheduling Service

[![CSharp](https://img.shields.io/badge/C%23-darkgreen?style=for-the-badge&logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)
[![.NET 10](https://img.shields.io/badge/.NET-10-5C2D91?style=for-the-badge)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-blueviolet?style=for-the-badge)](https://learn.microsoft.com/aspnet/core/)
[![EF Core](https://img.shields.io/badge/Entity_Framework_Core-8C3D65?style=for-the-badge)](https://learn.microsoft.com/ef/core/)
[![FluentValidation](https://img.shields.io/badge/FluentValidation-CC0000?style=for-the-badge)](https://docs.fluentvalidation.net/)
[![xUnit.net](https://img.shields.io/badge/XUNIT-F4F4F4?style=for-the-badge)](https://xunit.net/)
[![Moq](https://img.shields.io/badge/MOQ-050505?style=for-the-badge)](https://github.com/devlooped/moq)

[![MySQL](https://img.shields.io/badge/MYSQL-4479A1?style=for-the-badge)](https://www.mysql.com/)
[![Swagger](https://img.shields.io/badge/SWAGGER-GREEN?style=for-the-badge)](https://swagger.io/)

Esse projeto é um microserviço como **Application Programming Interface (API)** desenvolvido para gerenciar agendamentos de aulas vinculadas às mentorias existentes na plataforma **Menthfy**. O serviço permite que alunos solicitem aulas informando o período e o formato desejado, enquanto professores podem aprovar ou rejeitar as solicitações. Também permite a alteração de solicitações pendentes e o cancelamento de agendamentos conforme as regras de negócio.

Desenvolvida em **C#**, com **.NET 10**, **ASP.NET Core**, **Entity Framework Core** e **MySQL**, a aplicação utiliza **FluentValidation** para validação de dados e **Andras.Result** para representação dos resultados das operações. A documentação dos endpoints é disponibilizada por meio do **Swagger**, e os testes automatizados utilizam **xUnit** e **Moq**.

O projeto organiza suas responsabilidades em camadas com **Clean Architecture**, separando a API, os casos de uso, as regras de negócio, a persistência e os testes automatizados.

<p>
    <strong>Índice</strong>: &nbsp;&nbsp;
    <a href="#arquiteturas">Arquiteturas</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
    <a href="#tecnologias">Tecnologias</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
    <a href="#execucao">Execução</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
    <a href="#endpoints">Endpoints</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
    <a href="#testes">Testes</a>
</p>

<h2 id="arquiteturas">🏛️ Arquiteturas e responsabilidades</h2>

A solução está organizada em cinco projetos, com responsabilidades separadas para facilitar a manutenção, os testes e a evolução do sistema.

- **MentorshipScheduling.API**: contém os **Controllers**, responsáveis por receber as requisições HTTP e retornar as respostas da API.
- **MentorshipScheduling.Application**: coordena os **casos de uso** e as validações necessárias para as operações de agendamento.
- **MentorshipScheduling.Domain**: contém as **entidades**, os **estados dos agendamentos** e as regras de negócio relacionadas às transições de estado.
- **MentorshipScheduling.Infrastructure**: implementa a persistência de dados com **Entity Framework Core**, o acesso ao **MySQL**, os mapeamentos e os repositórios.
- **MentorshipScheduling.Tests**: contém os **testes automatizados** dos controllers, utilizando dependências simuladas para verificar os comportamentos HTTP.

O serviço utiliza a tabela `mentorships`, já existente no banco de dados da plataforma, para consultar as mentorias e verificar a associação entre os participantes.

As estruturas de alunos, professores e mentorias pertencem ao serviço preexistente e não são gerenciadas por esta aplicação.

Essa organização permite separar as regras de negócio dos detalhes de infraestrutura, facilitando a manutenção e a evolução do serviço.

<h2 id="tecnologias">🛠️ Tecnologias e Ferramentas Utilizadas</h2>

<div align="left">
  <table>
    <thead>
      <tr>
        <th>Tipo</th>
        <th>Ícone</th>
        <th>Tecnologia</th>
        <th>Descrição</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>💻 Linguagem</td>
        <td>
          <img height="30px" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg">
        </td>
        <td>C#</td>
        <td>Linguagem utilizada no desenvolvimento da aplicação e na implementação das regras de negócio.</td>
      </tr>
      <tr>
        <td>⚙️ Framework</td>
        <td>
          <img height="30px" src="https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/Microsoft_.NET_logo.svg/3840px-Microsoft_.NET_logo.svg.png">
        </td>
        <td>.NET 10 / ASP.NET Core</td>
        <td>Plataforma e framework utilizados para desenvolver e executar a API REST.</td>
      </tr>
      <tr>
        <td>🗃️ ORM</td>
        <td>
          <img height="30px" src="https://api.nuget.org/v3-flatcontainer/microsoft.entityframeworkcore/10.0.0/icon">
        </td>
        <td>Entity Framework Core</td>
        <td>Mapeamento objeto-relacional, consultas e persistência dos agendamentos no banco de dados.</td>
      </tr>
      <tr>
        <td>🐬 Banco de Dados</td>
        <td>
          <img height="30px" src="https://pngimg.com/uploads/mysql/mysql_PNG23.png">
        </td>
        <td>MySQL</td>
        <td>Sistema gerenciador de banco de dados relacional utilizado para armazenar os agendamentos e consultar as mentorias existentes.</td>
      </tr>
      <tr>
        <td>✅ Validação</td>
        <td>
          <img height="30px" src="https://api.nuget.org/v3-flatcontainer/fluentvalidation/12.1.1/icon">
        </td>
        <td>FluentValidation</td>
        <td>Biblioteca utilizada para validar os dados recebidos pelas operações da API.</td>
      </tr>
      <tr>
        <td>🔄 Result Pattern</td>
        <td>
          <img height="30px" src="https://avatars.githubusercontent.com/u/9919?s=200&v=4">
        </td>
        <td>Andras.Result</td>
        <td>Biblioteca utilizada para representar resultados de sucesso e falha nas operações da aplicação.</td>
      </tr>
      <tr>
        <td>📑 Documentação</td>
        <td>
          <img height="30px" src="https://cdn.jsdelivr.net/gh/homarr-labs/dashboard-icons/webp/swagger.webp">
        </td>
        <td>Swagger / Swashbuckle</td>
        <td>Ferramentas utilizadas para documentar, visualizar e testar os endpoints HTTP.</td>
      </tr>
      <tr>
        <td>🧪 Testes unitários</td>
        <td>
          <img height="30px" src="https://avatars.githubusercontent.com/u/2092016?s=280&v=4">
        </td>
        <td>xUnit.net</td>
        <td>Framework utilizado para executar os testes automatizados do projeto.</td>
      </tr>
      <tr>
        <td>🎭 Mock</td>
        <td>
          <img height="30px" src="https://avatars.githubusercontent.com/u/1434934?s=280&v=4">
        </td>
        <td>Moq</td>
        <td>Biblioteca utilizada para simular dependências e testar os controllers de forma isolada.</td>
      </tr>
    </tbody>
  </table>
</div>

<h2 id="execucao">▶️ Como executar o projeto</h2>

#### 📦 Pré-requisitos

- Sistema operacional compatível com .NET 10.
- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) instalado.
- MySQL instalado e em execução.

### 1. Clone o repositório

```bash
git clone https://github.com/FatecLP/mentorship-scheduling-service.git
cd mentorship-scheduling-service
```

### 2. Configure a conexão com o banco de dados

A aplicação utiliza a configuração `ConnectionStrings:MentorshipDatabase`.

Configure a connection string:

```json
{
    "ConnectionStrings": {
        "MentorshipDatabase": "Server=localhost;Port=3306;Database=menthfy_db;User=root;Password=root;"
    }
}
```

Ou no PowerShell, configure a variável de ambiente com a string de conexão correspondente ao seu ambiente:

```powershell
$env:ConnectionStrings__MentorshipDatabase = "Server=localhost;Port=3306;Database=menthfy_db;User=root;Password=SUA_SENHA;"
```

Ajuste o endereço, a porta, o usuário e a senha conforme a configuração do MySQL.

### 3. Execute as migrations

Na raiz da solução, a aplicação aplica as migrations pendentes durante a inicialização. Portanto, o usuário do banco de dados deve possuir as permissões necessárias para criar e alterar as tabelas de agendamento.

Caso não sejam aplicadas, execute:

```
cd .\MentorshipScheduling.Infrastructure
dotnet ef database update
```

### 4. Inicie a API

Execute o seguinte comando na raiz da solução:

```
dotnet watch --project .\MentorshipScheduling.API\MentorshipScheduling.API.csproj
```

Também é possível executar sem monitoramento de alterações:

```
dotnet run --project .\MentorshipScheduling.API\MentorshipScheduling.API.csproj
```

### 5. Acesso

Conforme o perfil configurado em `launchSettings.json`, a API estará disponível nos seguintes endereços:

- **HTTP**: http://localhost:5080
- **HTTPS**: https://localhost:7185
- **Swagger**: `/swagger` no endereço.

<h2 id="endpoints">📡 Endpoints</h2>

As rotas de agendamento utilizam o prefixo `/api/lesson-schedules`.

### Consultas

<table>
  <thead>
    <tr>
      <th>Método</th>
      <th>Rota</th>
      <th>Descrição</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>GET</td>
      <td><code>/api/lesson-schedules?studentId={id}</code></td>
      <td>Lista os agendamentos de um aluno.</td>
    </tr>
    <tr>
      <td>GET</td>
      <td><code>/api/lesson-schedules?teacherId={id}</code></td>
      <td>Lista os agendamentos de um professor.</td>
    </tr>
    <tr>
      <td>GET</td>
      <td><code>/api/lesson-schedules?studentId={id}&amp;mentorshipId={id}</code></td>
      <td>Lista os agendamentos de um aluno em uma mentoria específica.</td>
    </tr>
    <tr>
      <td>GET</td>
      <td><code>/api/lesson-schedules/{id}?studentId={id}</code></td>
      <td>Consulta um agendamento como aluno.</td>
    </tr>
    <tr>
      <td>GET</td>
      <td><code>/api/lesson-schedules/{id}?teacherId={id}</code></td>
      <td>Consulta um agendamento como professor.</td>
    </tr>
  </tbody>
</table>

### Gerenciamento de aulas

<table>
  <thead>
    <tr>
      <th>Método</th>
      <th>Rota</th>
      <th>Descrição</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>POST</td>
      <td><code>/api/lesson-schedules</code></td>
      <td>Cria uma solicitação de agendamento.</td>
    </tr>
    <tr>
      <td>PUT</td>
      <td><code>/api/lesson-schedules/{id}</code></td>
      <td>Altera uma solicitação pendente.</td>
    </tr>
    <tr>
      <td>POST</td>
      <td><code>/api/lesson-schedules/{id}/approve</code></td>
      <td>Aprova uma solicitação como professor.</td>
    </tr>
    <tr>
      <td>POST</td>
      <td><code>/api/lesson-schedules/{id}/reject</code></td>
      <td>Rejeita uma solicitação como professor.</td>
    </tr>
    <tr>
      <td>POST</td>
      <td><code>/api/lesson-schedules/{id}/cancel</code></td>
      <td>Cancela um agendamento como aluno ou professor.</td>
    </tr>
  </tbody>
</table>

### Exemplo de requisição

**Solicitar uma aula**

```http
POST /api/lesson-schedules
Content-Type: application/json
```

```json
{
  "mentorshipId": 12,
  "studentId": 23,
  "startAt": "2030-10-13T14:00:00-03:00",
  "endAt": "2030-10-13T15:00:00-03:00",
  "format": "Online"
}
```

O campo `mentorshipId` identifica a mentoria, `studentId` identifica o aluno solicitante, `startAt` e `endAt` definem o período da aula e `format` determina o formato do encontro.

As datas devem utilizar o formato ISO 8601 com indicação do fuso horário. O início deve estar no futuro e o término deve ser posterior ao início.

Os formatos disponíveis são `Online` e `InPerson`.

Uma solicitação criada com sucesso retorna `201 Created` e inicia no estado `PendingApproval`.

### Aprovar ou rejeitar uma solicitação

Para aprovar ou rejeitar uma solicitação, envie o identificador do professor no corpo da requisição:

```json
{
  "teacherId": 34
}
```

Utilize a rota `/approve` para aprovar ou `/reject` para rejeitar.

### Cancelar um agendamento

Para cancelar um agendamento como aluno, envie:

```json
{
  "studentId": 23
}
```

Para realizar o cancelamento como professor, utilize o campo `teacherId`.

### Estados dos agendamentos

Os agendamentos possuem os seguintes estados:

- `PendingApproval`: aguardando aprovação do professor.
- `Approved`: solicitação aprovada.
- `Rejected`: solicitação rejeitada.
- `Cancelled`: agendamento cancelado.

Somente solicitações pendentes podem ser alteradas, aprovadas ou rejeitadas. Agendamentos pendentes ou aprovados podem ser cancelados.

### Disponibilidade e tratamento de erros

Alunos e professores não podem possuir agendamentos ativos em horários sobrepostos. Agendamentos pendentes e aprovados reservam a disponibilidade.

<h2 id="testes">🧪 Testes</h2>

Os testes automatizados utilizam **xUnit** e **Moq** para verificar o comportamento dos controllers e simular suas dependências, sem exigir a inicialização do banco de dados para esses testes.

Na raiz da solução, execute:

```powershell
dotnet test
```

#

<div align="center">
  <strong>Desenvolvido para a plataforma Menthfy</strong>
</div>