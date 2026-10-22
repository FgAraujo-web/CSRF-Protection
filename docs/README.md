# POC | CSRF em ASP.NET Core MVC: um formulário legítimo não é apenas um POST

Uma aplicação pode validar a identidade de um utilizador e, ainda assim, aceitar um pedido que esse utilizador nunca quis executar.

É aqui que entra a proteção contra **Cross-Site Request Forgery (CSRF)**.

## O problema

Imaginemos uma aplicação de gestão com autenticação por cookies. Um administrador inicia sessão e, enquanto a sessão está ativa, visita uma página maliciosa. Essa página pode tentar desencadear um pedido para alterar dados na aplicação original. Dependendo das políticas de cookies e das condições do navegador, as credenciais podem acompanhar o pedido.

A autenticação confirma quem está associado à sessão, mas não demonstra, por si só, que o pedido teve origem numa interação legítima com a aplicação.

## A prova de conceito

Desenvolvi uma pequena aplicação em **C# e ASP.NET Core MVC** para demonstrar a diferença entre um pedido POST legítimo e outro sem token antifalsificação.

O projeto inclui um formulário de alteração de um nome fictício, um controlador MVC e uma configuração global de validação CSRF. Não utiliza base de dados nem autenticação real: o objetivo é isolar o mecanismo de proteção.

No `Program.cs`, a validação é configurada globalmente:

```csharp
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
```

No formulário Razor, o **Form Tag Helper** emite automaticamente o campo oculto `__RequestVerificationToken`. O servidor valida o token quando recebe um POST.

## O que acontece nos testes?

**Cenário 1 — formulário legítimo:** o navegador submete o nome com um token válido. O servidor aceita a operação e apresenta uma mensagem de sucesso.

**Cenário 2 — POST sem token:** um pedido enviado diretamente para a mesma ação, mas sem o token exigido, é rejeitado com **HTTP 400 Bad Request**. A alteração não é efetuada.

A diferença está no controlo de origem da operação, não apenas na validação dos dados submetidos.

## O que esta proteção não resolve

A antifalsificação CSRF não substitui autenticação, autorização por perfis, prevenção de XSS, validação de dados ou HTTPS. Um ataque XSS pode comprometer também as garantias do mecanismo antifalsificação. As políticas `SameSite` dos cookies constituem uma defesa complementar, não um substituto universal do token.

Além disso, os filtros MVC não protegem automaticamente todos os endpoints possíveis de uma aplicação. A configuração deve ser adequada à arquitetura utilizada.

## Conclusão

Na análise técnico-funcional de uma aplicação, não basta confirmar que uma operação existe e que o utilizador tem permissões para a executar. É igualmente importante verificar **como o pedido é validado antes de alterar o estado do sistema**.

Esta POC é um exemplo pequeno, mas ajuda a tornar visível um controlo de segurança que muitas vezes passa despercebido na interface.

**Tecnologias:** C# · ASP.NET Core MVC · Razor · HTTP · Segurança Aplicacional

**Código-fonte:** [adicionar ligação ao repositório após publicação]

#ASPNetCore #CSharp #CyberSecurity #AppSec #CSRF #SegurancaAplicacional


## Evidência da POC (testes locais, 22/10/2026)

Os três cenários foram observados no Chrome: submissão com token válido (alteração aceite), sem token (HTTP 400) e com token adulterado (HTTP 400). As capturas podem ser acrescentadas ao repositório para permitir a consulta do Payload e do código de resposta.

**Nota técnica:** esta demonstração não utiliza autenticação; o risco CSRF é particularmente relevante em aplicações que enviam automaticamente credenciais, como cookies de sessão.
