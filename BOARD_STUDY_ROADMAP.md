# 게시판 백엔드 단계학습 로드맵

> 원칙: 이 문서는 **정답 코드를 주지 않는다.**
> 각 페이즈마다 "무엇을 / 왜 / 힌트"만 보고 직접 구현한다.
> 막히면 키워드를 검색하고, 다음 페이즈로 넘어가기 전 "완료 기준"을 반드시 통과할 것.
>
> 현재 출발점 (2026-09-30 기준):
> - `net10.0` + `ImplicitUsings` + `Nullable enable`
> - `Program.cs`: Minimal API `MapGet("/")` 하나만 존재
> - `models/post.cs`: `post` 클래스, `id/title/content/createAt` (네이밍/네임스페이스 미정리 상태)

---

## Phase 0 — 현재 코드 진단하기 (준비 운동)

**목표:** 지금 코드의 문제점을 스스로 찾기

**왜:** 게시판 만들기 전에 "고칠 곳"을 알아야 학습이 된다.

**해야 할 일:**
1. `models/post.cs`를 읽고 아래를 고민해보기
   - 클래스명/속성명이 C# 관례(PascalCase)에 맞는가?
   - `namespace`가 없으면 나중에 어떤 문제가 생길까?
   - `DateTime.Now` vs `DateTime.UtcNow`, 뭐가 맞을까?
   - `title`, `content`에 `required`, `string.Empty` 중 뭐가 어울릴까?
2. `Program.cs` 구조 파악하기
   - `builder`와 `app`의 역할 차이는?
   - `MapGet`은 어떤 일을 하는가?

**힌트 키워드:**
- `C# naming conventions`, `C# namespace`, `ImplicitUsings`, `Nullable reference types`

**완료 기준:**
- [ ] `post` → 어떻게 고쳐야 할지 말로 설명할 수 있다
- [ ] `dotnet run` 후 `http://localhost:5000/` 접속 시 "Hello World!" 확인

**다음으로 넘어가기 전 자가체크:**
- Q. 지금 `post` 클래스를 그대로 쓰면 협업 시 어떤 혼란이 생길까?

---

## Phase 1 — 모델 다듬기 (Post 바로잡기)

**목표:** 게시판의 핵심 엔티티 `Post`를 제대로 정의

**왜:** 모델이 흔들리면 뒤의 모든 CRUD가 흔들린다.

**해야 할 일:**
1. `models/` 폴더/파일명 정리 (대소문자, 복수/단수 고민)
2. 클래스/속성명을 PascalCase로 (`Post`, `Id`, `Title`, `Content`, `CreatedAt`)
3. `namespace` 추가 (예: 프로젝트명 기준)
4. 게시판에 필요한 필드 1~2개 추가해보기 (힌트: 작성자? 수정일? 조회수?)
   - 추가할 때 타입과 기본값을 왜 그렇게 정했는지 주석으로 남기기

**힌트 (코드는 직접 작성):**
- 파일 구조 힌트: `namespace xxx.Models;` 한 줄로도 충분하다
- 속성 힌트: `get; set;` 뒤에 `= ...;` / `{ get; }` 차이를 찾아볼 것
- 시간 힌트: `DateTime.UtcNow`를 검색해보고 `Now`와 비교

**하지 말 것:**
- ❌ EF Core, DB 연결 아직 하지 말 것
- ❌ DTO를 아직 만들지 말 것 (Phase 3에서 한다)

**완료 기준:**
- [ ] `dotnet build` 성공
- [ ] `Post` 클래스의 각 속성이 왜 그 타입/이름인지 설명 가능

**자가체크:**
- Q. `CreatedAt`에 `set`을 열어둘까, 생성자에서만 세팅할까?

---

## Phase 2 — In-Memory CRUD API (DB 없이 게시판 동작시키기)

**목표:** DB 없이 `List<Post>`만으로 CRUD 완성

**왜:** HTTP + 라우팅 + 상태코드를 가장 빨리 익히는 방법. DB 문제를 분리한다.

**해야 할 일:**
1. `Program.cs`에 아래 5개 엔드포인트 추가:
   - `GET /posts` — 전체 조회
   - `GET /posts/{id}` — 단건 조회 (없으면 404)
   - `POST /posts` — 생성 (201 + 생성된 객체 반환)
   - `PUT /posts/{id}` — 수정 (없으면 404)
   - `DELETE /posts/{id}` — 삭제 (없으면 404, 성공 시 204)
2. 저장소는 `List<Post>` 하나 (어디에 선언할지 고민)
3. `id` 자동 증가 로직 직접 만들기 (힌트: static 변수? Max+1?)

**힌트 (찾아볼 것):**
- `Minimal API MapGet MapPost MapPut MapDelete`
- `Results.Ok / Results.NotFound / Results.Created / Results.NoContent` 차이
- `app.MapPost("/posts", ([FromBody] ... ) => ...)` 형태가 왜 필요한지
- 테스트 도구: `curl`, `httpie`, 또는 VSCode `REST Client` 중 하나

**하지 말 것:**
- ❌ NuGet 패키지 설치 금지 (EF Core는 Phase 4)
- ❌ 파일 분리 금지 — 일단 `Program.cs`에 다 몰아넣고 "지저분함"을 느껴볼 것 (Phase 7에서 나눈다)

**완료 기준:**
- [ ] 서버 재시작하면 데이터 날아가는 것을 확인 (정상이다)
- [ ] 존재하지 않는 id 조회/수정/삭제 시 404 반환 확인
- [ ] `curl` 또는 브라우저로 5개 API 전부 호출 성공

**자가체크:**
- Q. `List<Post>`를 여러 요청이 동시에 건드리면 무슨 일이 생길까? (정답 몰라도 됨, 느껴보기)

---

## Phase 3 — DTO + Validation (입력/출력 분리)

**목표:** `Post` 그대로 주고받지 않고, 용도별 DTO 만들기

**왜:** "생성 요청"과 "조회 응답"이 같으면 안 되는 이유를 배우는 단계.

**해야 할 일:**
1. 아래 DTO를 왜 나누는지 고민하고 직접 정의:
   - 생성용 (힌트: `Id`, `CreatedAt`이 필요할까?)
   - 수정용 (힌트: 생성용과 같아도 될까?)
   - 응답용 (힌트: 엔티티 그대로 내보내도 될까?)
2. 유효성 검사 추가:
   - 제목/내용이 비어있으면 400으로 거절
   - 제목 길이 제한 걸어보기 (예: 100자)
3. 검사 실패 시 에러 메시지 형태 통일해보기

**힌트 키워드:**
- `C# record vs class DTO`, `Minimal API validation`, `Results.BadRequest`, `string.IsNullOrWhiteSpace`
- 여유되면: `FluentValidation`, `DataAnnotations [Required] [MaxLength]`

**완료 기준:**
- [ ] 빈 제목으로 POST → 400 확인
- [ ] DTO 없이 `Post`를 직접 받던 코드와 비교해서 "왜 DTO가 낫는지" 한 줄로 설명 가능

**자가체크:**
- Q. 수정(PUT)할 때 제목만 바꾸고 싶으면 어떻게 해야 할까? (PATCH를 찾아보게 되는 질문)

---

## Phase 4 — EF Core + SQLite로 영속화 (재시작해도 유지)

**목표:** `List<Post>`를 진짜 DB로 교체

**왜:** 게시판의 핵심이 "저장"이다. 여기서 ORM과 마이그레이션을 배운다.

**해야 할 일:**
1. 패키지 설치 (힌트: 딱 2개면 된다 — `EfCore`, `Sqlite` 키워드로 검색)
2. `DbContext` 클래스 만들기 (힌트: `DbSet<Post>`가 왜 필요한지)
3. `Program.cs`에 DbContext 등록 (힌트: `AddDbContext`, `UseSqlite`, `appsettings.json` 연결문자열)
4. 마이그레이션 + DB 생성 명령 실행 (힌트: `dotnet ef`가 없으면? `dotnet tool install` 키워드)
5. Phase 2의 `List<Post>` 로직을 `DbContext` 기반으로 하나씩 교체

**힌트 키워드:**
- `EF Core DbContext`, `AddDbContext UseSqlite`, `dotnet ef migrations add InitialCreate`, `dotnet ef database update`
- 연결문자열 힌트: `appsettings.json`에 `"ConnectionStrings": { "Default": "Data Source=board.db" }` 형태가 일반적이다 — 왜 `appsettings`에 두는지 고민

**하지 말 것:**
- ❌ Phase 2 코드를 한 번에 다 지우지 말 것 — 엔드포인트 하나씩 바꿔가며 테스트
- ❌ `EnsureCreated` vs `Migrate` 차이를 모르면 넘어가지 말 것 (검색 5분)

**완료 기준:**
- [ ] 서버 재시작해도 글이 남아있다
- [ ] `board.db` 파일 생성 확인
- [ ] `GET /posts`가 DB에서 읽어오는 것을 확인

**자가체크:**
- Q. `DbContext`의 생명주기(Scoped)가 뭔지 말할 수 있는가?

---

## Phase 5 — 조회 고도화 (페이징 + 검색 + 정렬)

**목표:** 글이 1000개 쌓여도 터지지 않는 목록 API

**왜:** 실무 게시판과 장난감 게시판의 차이다.

**해야 할 일:**
1. `GET /posts`에 쿼리스트링 추가:
   - `?page=1&pageSize=10` (힌트: `Skip`, `Take`)
   - `?q=키워드` (힌트: `Where + Contains`, 제목+내용 검색)
   - `?sort=latest` 또는 `oldest` (힌트: `OrderByDescending`)
2. 전체 개수와 함께 반환할지 고민 (힌트: 프론트가 페이지 버튼을 그리려면 뭐가 필요할까?)
3. `pageSize` 상한선 걸기 (힌트: 누군가 `pageSize=1000000`을 보내면?)

**힌트 키워드:**
- `LINQ Skip Take`, `EF Core Where Contains`, `Minimal API query string binding`
- 응답 형태 힌트: `{ totalCount, page, pageSize, items: [...] }` — 왜 이렇게 감싸는지 고민

**완료 기준:**
- [ ] 글 20개 넣고 `page=2&pageSize=10`이 정확히 10개 반환
- [ ] 검색어 없을 때와 있을 때 결과가 다름을 확인
- [ ] `pageSize=99999` 요청 시 서버가 죽지 않음

**자가체크:**
- Q. `ToList()`를 `Skip/Take`보다 먼저 호출하면 왜 느려질까?

---

## Phase 6 — 에러 처리 + 상태코드 정리 (마무리 품질)

**목표:** "되는 API"에서 "믿을 수 있는 API"로

**왜:** 프론트/협업자가 제일 먼저 보는 게 에러 메시지다.

**해야 할 일:**
1. 존재하지 않는 글 / 잘못된 입력 / 서버 에러를 구분해서 반환
2. `try-catch`를 각 엔드포인트에 다 넣지 말고 모으는 방법 고민 (힌트: `ExceptionHandler`, `UseExceptionHandler`)
3. 개발 중에 요청/응답을 눈으로 보는 도구 붙이기 (힌트: `Swagger`, `Swashbuckle`, `Scalar`)
4. `createAt` 네이밍 같은 잔재 정리 + `Program.cs`가 너무 길면 주석으로 구역 나누기

**힌트 키워드:**
- `ASP.NET Core exception handling middleware`, `Results.Problem`, `OpenAPI Swagger Minimal API`
- 상태코드 힌트: `200 / 201 / 204 / 400 / 404` — 각자 언제 쓰는지 표로 정리해보기

**완료 기준:**
- [ ] 잘못된 요청 3종(없는 id, 빈 제목, 잘못된 page)을 보내고 각각 다른 상태코드 확인
- [ ] Swagger(또는 동등 도구) 화면에서 5개 API가 전부 보임
- [ ] 친구에게 "이 API 어떻게 쓰는지" 3분 안에 설명 가능

**자가체크:**
- Q. 500 에러가 났을 때 스택트레이스를 그대로 클라이언트에 줘도 될까?

---

## Phase 7 — (심화, 선택) 구조 나누기 + 테스트

> 여기부터는 시간 남을 때만. Phase 6까지가 게시판 백엔드 본체다.

**7-A. 레이어 분리:**
- `Endpoints/`, `Services/`, `Repositories/` 로 나누기
- `Program.cs`는 등록만, 로직은 서비스로 (힌트: `DI AddScoped`, `MapGroup("/posts")`)
- 왜 나누는지 "테스트" 관점에서 생각해보기

**7-B. 테스트 1개만:**
- `xUnit` + `WebApplicationFactory` 키워드 검색
- `POST` 후 `GET`으로 확인하는 테스트 하나만 작성
- 통과하면 충분하다

**7-C. 다음 학습 제안:**
- 인증 (작성자 본인만 수정/삭제 — `JWT` 키워드)
- 댓글 (1:N 관계 — `EF Core navigation property`, `Include`)
- 조회수 (동시성 — `concurrency token`)

---

## 학습 규칙

1. **한 페이즈에 하루 이상 쓰지 말기** — 막히면 30분 고민 후 질문/검색
2. **매 페이즈 끝날 때 커밋** — 메시지 예: `phase2: in-memory CRUD 완성`
3. **복붙 금지** — 이해 안 되는 코드는 지우고 손으로 다시 치기
4. **이 문서에 정답 코드를 추가하지 말기** — 힌트만 추가 가능

## 진행 체크표

- [ ] Phase 0 — 진단
- [ ] Phase 1 — 모델
- [ ] Phase 2 — In-Memory CRUD
- [ ] Phase 3 — DTO + Validation
- [ ] Phase 4 — EF Core + SQLite
- [ ] Phase 5 — 페이징/검색/정렬
- [ ] Phase 6 — 에러/Swagger
- [ ] Phase 7 — 심화 (선택)
