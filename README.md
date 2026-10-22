# CSRF Protection Demo | ASP.NET Core MVC

POC didática para demonstrar a proteção antifalsificação (CSRF) em formulários ASP.NET Core MVC.

## Requisitos
- .NET 10 SDK (ou SDK compatível que permita compilar `net10.0`)
- Navegador web

## Executar
```bash
dotnet restore
dotnet run
```
Abre o endereço indicado no terminal (`localhost`). Se for apresentado um aviso de certificado HTTPS no ambiente de desenvolvimento, utiliza `dotnet dev-certs https --trust` quando adequado.

## Experiência 1 — pedido legítimo
1. Abre a página inicial e altera o nome fictício.
2. Carrega em **Guardar alteração**.
3. A aplicação apresenta uma mensagem de sucesso e o valor atualizado.
4. Inspeciona o HTML do formulário: o Form Tag Helper inclui `__RequestVerificationToken` automaticamente.

## Experiência 2 — pedido sem token
No navegador, abre as ferramentas de programador, separador **Network/Rede**, e identifica o endereço HTTPS local e o POST para `/Home/Save`.

Num terminal, executa (substitui a porta pela indicada na aplicação):
```bash
curl -k -i -X POST https://localhost:PORTA/Home/Save \
  -H "Content-Type: application/x-www-form-urlencoded" \
  --data-urlencode "Name=AlteracaoSemToken"
```
**Resultado esperado:** `HTTP/1.1 400 Bad Request` (ou `HTTP/2 400`). O nome guardado não é alterado.

Nota: o comando acima é apenas para localhost; `-k` ignora a validação do certificado TLS para testes locais. Não o utilizes para sistemas reais.

## Experiência 3 — token adulterado
1. Reabre a página inicial e, em **F12 → Elements**, localiza o campo oculto `__RequestVerificationToken`.
2. Substitui apenas o atributo `value` por `TOKEN_INVALIDO_TESTE_CSRF`.
3. Submete o formulário e verifica **HTTP 400 Bad Request**.
4. Em **Network → Payload**, confirma que o campo existe mas contém o valor adulterado.

## Resultados observados nos testes locais (22/10/2026)

| Cenário | Payload | Resposta observada |
|---|---|---|
| Token válido | Campo presente, token original | Alteração aceite; POST 302 → GET 200 |
| Token ausente | Apenas `Name=Teste sem CSRF` | POST 400 Bad Request |
| Token adulterado | `__RequestVerificationToken=TOKEN_INVALIDO_TESTE_CSRF` | POST 400 Bad Request |

Os testes foram executados localmente no Chrome com o projeto aberto no Visual Studio. O código HTTP 400 isoladamente não identifica a causa; o separador Payload e a configuração do filtro antifalsificação permitem relacioná-lo com o token em falta ou inválido. A POC não foi submetida a uma auditoria de segurança completa.

### Capturas de ecrã sugeridas
- `docs/imagens/01-token-valido.png`: mensagem de alteração aceite.
- `docs/imagens/02-token-ausente.png`: Network → Payload sem `__RequestVerificationToken` e resposta 400.
- `docs/imagens/03-token-adulterado.png`: Network → Payload com token alterado e resposta 400.

As imagens devem ser adicionadas ao repositório após exportação do navegador; não estão incluídas neste pacote.

## Onde está a proteção?
- `Program.cs`: registo global de `AutoValidateAntiforgeryTokenAttribute`.
- `Views/Home/Index.cshtml`: formulário Razor com Form Tag Helper que emite o token.
- `Controllers/HomeController.cs`: ação POST protegida pelo filtro global.

## Limitações e boas práticas
- A POC **não inclui autenticação**, autorização, base de dados ou persistência. O armazenamento em memória é partilhado e reinicia ao parar a aplicação.
- CSRF é particularmente relevante em aplicações autenticadas por cookies; não substitui autenticação, autorização, proteção XSS, validação de entrada ou HTTPS.
- Evita alterações de estado em pedidos GET. Em APIs, a abordagem depende do mecanismo de autenticação e de como as credenciais são enviadas.
- O filtro global protege métodos HTTP inseguros em ações MVC; endpoints fora do MVC precisam de configuração própria.
- Não exponhas este projeto como aplicação de produção.

## Estrutura
```text
CSRF-Protection-Demo/
├── Program.cs
├── CSRFProtectionDemo.csproj
├── Controllers/HomeController.cs
├── Models/ProfileInput.cs
├── Services/DemoStore.cs
├── Views/_ViewImports.cshtml
├── Views/Home/Index.cshtml
└── docs/artigo-linkedin.md
```

## Referências
- Microsoft Learn — [Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core](https://learn.microsoft.com/aspnet/core/security/anti-request-forgery)
- OWASP — [Cross-Site Request Forgery Prevention Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Cross-Site_Request_Forgery_Prevention_Cheat_Sheet.html)
