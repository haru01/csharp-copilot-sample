# C# Backend with DDD and Value Objects - GitHub Copilot Development Sample

## プロジェクト概要

このプロジェクトは、GitHub Copilotを活用したC#バックエンド開発のサンプルです。Domain-Driven Design (DDD)、Entity Framework Core、xUnit、および値オブジェクト（Value Objects）を使用し、TDD（テスト駆動開発）のアプローチでAPIを構築します。実際の企業開発で使われるベストプラクティスを体現し、GitHub Copilotの効果的な活用方法を示しています。

## 技術スタック

- **言語**: C# (.NET 9)
- **フレームワーク**: ASP.NET Core Web API
- **アーキテクチャ**: Domain-Driven Design (DDD)、Clean Architecture
- **ORM**: Entity Framework Core
- **データベース**: SQLite (開発用) / InMemory (テスト用)
- **テスト**: xUnit, Entity Framework InMemory
- **バリデーション**: FluentValidation
- **設計パターン**: Repository Pattern, Service Layer, Value Objects
- **その他**: OpenAPI (Swagger)
- **開発ツール**: GitHub Copilot

## DDD レイヤー構造

```
csharp-copilot/
├── src/
│   └── CopilotSample.Api/          # Web APIプロジェクト
│       ├── Domain/                 # ドメイン層（最重要層）
│       │   ├── Entities/           # ドメインエンティティ
│       │   │   ├── Product.cs      # 商品集約ルート（リッチドメインモデル）
│       │   │   └── Category.cs     # カテゴリエンティティ
│       │   └── Values/             # 値オブジェクト
│       │       ├── Money.cs        # 価格値オブジェクト（通貨対応）
│       │       ├── ProductSKU.cs   # SKU値オブジェクト（検証・正規化）
│       │       └── StockQuantity.cs # 在庫値オブジェクト（ビジネスルール）
│       ├── Application/            # アプリケーション層
│       │   ├── Services/           # アプリケーションサービス
│       │   │   ├── IProductService.cs # サービスインターフェース
│       │   │   └── ProductService.cs  # ビジネスロジック調整
│       │   ├── Interfaces/         # 依存性逆転のためのインターフェース
│       │   │   └── IProductRepository.cs # リポジトリ抽象化
│       │   └── DTOs/               # データ転送オブジェクト
│       │       └── CreateProductDto.cs # API契約
│       ├── Infrastructure/         # インフラストラクチャ層
│       │   ├── Data/               # データアクセス
│       │   │   └── AppDbContext.cs # EFコンテキスト（値オブジェクト変換設定）
│       │   └── Repositories/       # リポジトリ実装
│       │       └── ProductRepository.cs # データアクセス実装
│       ├── Controllers/            # プレゼンテーション層
│       │   └── ProductsController.cs # HTTP API エンドポイント
│       ├── Program.cs              # アプリケーションエントリポイント
│       └── CopilotSample.Api.http  # HTTPリクエストテンプレート
├── tests/
│   └── CopilotSample.Tests/       # xUnitテストプロジェクト
│       ├── Repositories/           # リポジトリテスト（8テスト）
│       │   └── ProductRepositoryTests.cs
│       ├── Services/               # サービステスト（12テスト）
│       │   └── ProductServiceTests.cs
│       └── Helpers/                # テストヘルパー
│           ├── TestDbContextFactory.cs # インメモリDB設定
│           ├── ProductBuilder.cs   # 商品テストデータビルダー
│           └── CategoryBuilder.cs  # カテゴリテストデータビルダー
├── .github/
│   └── copilot-instructions.md    # GitHub Copilot指示書
├── CopilotSample.sln              # ソリューションファイル
├── README.md                       # プロジェクト概要
└── CLAUDE.md                       # このファイル（開発者ガイド）
```

## DDD主要概念の実装

### 1. ドメイン層（Domain Layer）

#### エンティティ（Entities）
- **Product.cs**: 商品集約ルート
  - リッチドメインモデルとして実装
  - ファクトリメソッド（`Create`）でオブジェクト生成
  - ビジネスメソッド（`AddStock`, `DeductStock`, `IsInStock`等）
  - 値オブジェクトを活用した型安全性

#### 値オブジェクト（Value Objects）
- **Money.cs**: 通貨付き金額値オブジェクト
  - 通貨とその額の組み合わせ
  - 算術演算、比較演算のサポート
  - 型安全な価格操作

- **ProductSKU.cs**: SKU値オブジェクト
  - 形式検証（英数字・ハイフン、3-50文字）
  - 自動正規化（大文字変換、トリム）
  - プレフィックス操作メソッド

- **StockQuantity.cs**: 在庫量値オブジェクト
  - 負の値の防止
  - 在庫操作メソッド（追加、減算、充足性チェック）
  - ビジネスルール封じ込め

### 2. アプリケーション層（Application Layer）

#### 依存性逆転原則
- **IProductRepository**: インフラ層への依存を抽象化
- アプリケーション層がドメイン層のみに依存
- インフラ層がアプリケーション層のインターフェースを実装

#### アプリケーションサービス
- **ProductService**: ビジネスフロー調整
- ドメインメソッドの組み合わせ
- トランザクション境界の定義

### 3. インフラストラクチャ層（Infrastructure Layer）

#### Entity Framework設定
- 値オブジェクトのデータベース変換設定
- ValueComparerによる適切な比較設定
- リレーショナルマッピング

```csharp
// 値オブジェクトのEF設定例
entity.Property(p => p.SKU)
    .HasConversion(
        v => v.Value,           // DB保存時は文字列
        v => ProductSKU.Create(v), // 読み込み時は値オブジェクト
        new ValueComparer<ProductSKU>(
            (l, r) => l.Value == r.Value,
            v => v.Value.GetHashCode(),
            v => ProductSKU.Create(v.Value)))
```

## GitHub Copilot活用のポイント

### 1. DDD指向のコメント

```csharp
// TODO: ドメインエンティティにビジネスロジックを追加
// - 在庫充足性の検証
// - 価格範囲の妥当性チェック
// - SKUフォーマットの検証
// 値オブジェクトのメソッドを活用してドメインルールを実装
public bool ValidateForSale()
{
    // Copilotがドメインロジックを提案
}
```

### 2. 値オブジェクト中心の設計

```csharp
// TODO: 値オブジェクトを使った型安全な商品作成
// - Money値オブジェクトで価格を指定
// - ProductSKU値オブジェクトでSKU検証
// - StockQuantity値オブジェクトで在庫管理
// ファクトリメソッドで不正な状態を防ぐ
public static Product Create(string name, Money price, ProductSKU sku, StockQuantity stock)
{
    // Copilotが値オブジェクトを活用したファクトリを提案
}
```

### 3. レイヤー境界の明確化

```csharp
// Application層 - ドメインサービスの調整
// TODO: 複数のドメインオブジェクトを協調させるアプリケーションサービス
// - 在庫確認から減算までの一連のフロー
// - ドメインイベントの発行
// - トランザクション境界の管理
public async Task<bool> ProcessOrderAsync(OrderRequest request)
{
    // Copilotがアプリケーションフローを提案
}
```

## テスト戦略（TDD with DDD）

### 1. ドメインロジックのテスト

```csharp
[Fact]
public void 在庫が十分な場合にtrueを返す()
{
    // Arrange - 値オブジェクトを使ったテストデータ
    var stock = StockQuantity.Create(10);
    var product = Product.Create("テスト商品", Money.FromDecimal(1000), 
                                 ProductSKU.Create("TEST-001"), 1, stock);
    
    // Act - ドメインメソッドのテスト
    var result = product.IsInStock(5);
    
    // Assert
    Assert.True(result);
}
```

### 2. 値オブジェクトのテスト

```csharp
[Theory]
[InlineData("test-001", "TEST-001")]  // 大文字変換
[InlineData(" PROD-123 ", "PROD-123")] // トリム
public void SKU正規化が正しく動作する(string input, string expected)
{
    // Arrange & Act
    var sku = ProductSKU.Create(input);
    
    // Assert
    Assert.Equal(expected, sku.Value);
}
```

### 3. 統合テスト（InMemory DB）

```csharp
[Fact]
public async Task 値オブジェクトがEFで正しく永続化される()
{
    // Arrange - 値オブジェクトを含む商品作成
    var product = Product.Create("商品", Money.FromDecimal(500), 
                                 ProductSKU.Create("TEST-001"), 1, 
                                 StockQuantity.Create(20));
    
    // Act - データベース保存
    var saved = await _repository.AddAsync(product);
    
    // Assert - 値オブジェクトの復元確認
    Assert.Equal(500m, saved.Price.Amount);
    Assert.Equal("TEST-001", saved.SKU.Value);
    Assert.Equal(20, saved.Stock.Value);
}
```

## 実装されている機能詳細

### ドメインモデル（リッチドメインモデル）

#### Product（商品集約）
- **ファクトリメソッド**: 値オブジェクトを使った安全な生成
- **値オブジェクト活用**: Price（Money）、SKU（ProductSKU）、Stock（StockQuantity）
- **ビジネスメソッド**: 
  - `IsInStock(quantity)` - 在庫充足性チェック
  - `AddStock(quantity)` - 在庫追加
  - `DeductStock(quantity)` - 在庫減算  
  - `CalculateInventoryValue()` - 在庫価値計算
  - `IsLowStock(threshold)` - 低在庫判定
- **不変性**: プライベートセッターによる状態保護

#### Category（カテゴリ）
- 基本的なエンティティ実装
- Productとの一対多関係

### 値オブジェクト（Type Safety & Business Rules）

#### Money値オブジェクト
```csharp
public readonly record struct Money(decimal Amount, string Currency = "JPY")
{
    public static Money FromDecimal(decimal amount, string currency = "JPY");
    public Money Add(Money other);
    public Money Multiply(int quantity);
    // 通貨別の表示形式、演算オーバーロード等
}
```

#### ProductSKU値オブジェクト
```csharp
public readonly record struct ProductSKU(string Value)
{
    public static ProductSKU Create(string value); // 検証・正規化
    public bool HasPrefix(string prefix);
    public string GetPrefix();
    // 形式検証、プレフィックス操作等
}
```

#### StockQuantity値オブジェクト
```csharp
public readonly record struct StockQuantity(int Value)
{
    public static StockQuantity Create(int value); // 負の値防止
    public StockQuantity Add(int quantity);
    public StockQuantity Deduct(int quantity);
    public bool IsSufficient(int requiredQuantity);
    // 在庫操作、ビジネスルール封じ込め
}
```

### アプリケーションサービス

#### ProductService
- **ドメインロジックの調整**: 複数エンティティの協調
- **値オブジェクト活用**: `(decimal)product.CalculateInventoryValue()` 
- **検索機能**: 名前・SKUでの部分一致検索
- **在庫管理**: ドメインメソッドへのデリゲート

### データアクセス層

#### Entity Framework設定
- **値オブジェクト変換**: ドメインオブジェクト ↔ プリミティブ型
- **ValueComparer**: 変更追跡の最適化
- **制約設定**: ユニーク制約、精度設定等

## コマンドリファレンス

### プロジェクト管理
```bash
# プロジェクトのビルド
dotnet build

# テスト実行（全20テスト）
dotnet test

# テスト詳細表示
dotnet test --verbosity normal

# 特定テストクラス実行
dotnet test --filter "ClassName=ProductRepositoryTests"

# カバレッジ取得
dotnet test --collect:"XPlat Code Coverage"
```

### 開発フロー
```bash
# 1. テスト作成・実行（TDD）
dotnet test --filter "新機能テスト"

# 2. 実装
# ドメイン層 → アプリケーション層 → インフラ層

# 3. 統合テスト
dotnet test

# 4. API実行確認
dotnet run --project src/CopilotSample.Api
```

## アーキテクチャ設計思想

### 1. ドメイン中心設計
- **ドメイン層が最重要**: ビジネスルールの集中管理
- **依存性逆転**: 外部レイヤーがドメインに依存
- **純粋性維持**: ドメイン層は外部フレームワークに非依存

### 2. 値オブジェクトによる型安全性
- **プリミティブ執着の解消**: `decimal price` → `Money price`
- **不正状態の排除**: コンパイル時での型チェック
- **ビジネスルールの局所化**: 関連ロジックの値オブジェクト内配置

### 3. リッチドメインモデル
- **貧血症モデルの回避**: エンティティ内にビジネスロジック配置
- **ファクトリメソッド**: 適切な初期化の強制
- **封じ込め**: プライベートセッターによる不変性確保

### 4. テスト駆動開発
- **統合テスト中心**: InMemoryデータベースでの実環境テスト
- **モック排除**: 実オブジェクトによる信頼性向上
- **Builder Pattern**: 一貫したテストデータ作成

## 今後の拡張案

### ドメイン機能
- **集約境界の拡大**: Order、Customer等の新集約
- **ドメインイベント**: 状態変更時の非同期処理
- **仕様パターン**: 複雑な検索条件の抽象化

### 値オブジェクト
- **Address**: 住所値オブジェクト（郵便番号、都道府県等）
- **Email**: メールアドレス値オブジェクト（検証付き）
- **DateRange**: 期間値オブジェクト（開始・終了日付）

### アーキテクチャ
- **CQRS**: 読み書き分離による性能向上
- **Event Sourcing**: イベント履歴による状態管理
- **マイクロサービス**: ドメイン境界での分割

## FluentValidation による統一バリデーション

### FluentValidation 設計思想
- **宣言的バリデーション**: 流れるような記述でバリデーションルールを定義
- **値オブジェクトとの統合**: 値オブジェクトの検証ロジックと連携
- **統一されたエラーハンドリング**: 一貫したバリデーション例外処理
- **テスト可能性**: バリデーションルールの独立テスト

### 値オブジェクトの FluentValidation 統合

#### Money 値オブジェクトのバリデーション
```csharp
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("価格は0より大きい値を入力してください");
            
        RuleFor(x => x.Currency)
            .Must(BeValidCurrency)
            .WithMessage("対応していない通貨です");
    }
    
    private bool BeValidCurrency(string currency)
    {
        // Money値オブジェクトでサポートされている通貨かチェック
        return Money.IsSupportedCurrency(currency);
    }
}
```

#### ProductSKU 値オブジェクトのバリデーション
```csharp
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.SKU)
            .NotEmpty()
            .WithMessage("SKUは必須です")
            .Must(BeValidSKU)
            .WithMessage("SKU形式が正しくありません（英数字・ハイフン、3-50文字）");
    }
    
    private bool BeValidSKU(string sku)
    {
        try
        {
            // ProductSKU値オブジェクトの検証ロジックを活用
            ProductSKU.Create(sku);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

#### StockQuantity 値オブジェクトのバリデーション
```csharp
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.InitialStock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("初期在庫は0以上の値を入力してください");
    }
}
```

### API レイヤーでの統一バリデーション

#### コントローラーでのバリデーション実装
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IValidator<CreateProductDto> _validator;
    private readonly IProductService _productService;
    
    public ProductsController(
        IValidator<CreateProductDto> validator,
        IProductService productService)
    {
        _validator = validator;
        _productService = productService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto)
    {
        // FluentValidation による事前検証
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        try
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            // 値オブジェクトのバリデーションエラー
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

### テスト戦略（FluentValidation）

#### バリデーターのテスト
```csharp
public class CreateProductDtoValidatorTests
{
    private readonly CreateProductDtoValidator _validator;
    
    public CreateProductDtoValidatorTests()
    {
        _validator = new CreateProductDtoValidator();
    }
    
    [Fact]
    public void Should_Have_Error_When_Price_Is_Zero()
    {
        // Arrange
        var dto = new CreateProductDto { Price = 0 };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateProductDto.Price));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("INVALID-SKU!")]
    [InlineData("A")]
    public void Should_Have_Error_When_SKU_Is_Invalid(string invalidSku)
    {
        // Arrange
        var dto = new CreateProductDto { SKU = invalidSku };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateProductDto.SKU));
    }
}
```

### 依存性注入設定

#### Program.cs での FluentValidation 設定
```csharp
// FluentValidation の設定
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();

// またはマニュアル登録
builder.Services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
```

### エラーハンドリングの統一

#### グローバル例外ハンドラー
```csharp
public class GlobalExceptionHandler
{
    public static IResult HandleValidationException(ValidationException ex)
    {
        var errors = ex.Errors.Select(e => new 
        {
            PropertyName = e.PropertyName,
            ErrorMessage = e.ErrorMessage
        });
        
        return Results.BadRequest(new { errors });
    }
    
    public static IResult HandleDomainException(ArgumentException ex)
    {
        // 値オブジェクトのバリデーションエラー
        return Results.BadRequest(new { error = ex.Message });
    }
}
```

## 重要な設計原則

1. **Single Responsibility Principle**: 各クラスは単一の責任
2. **Open/Closed Principle**: 拡張に開いて、修正に閉じている
3. **Dependency Inversion**: 高レベルモジュールは低レベルモジュールに非依存
4. **Domain-Driven Design**: ドメインエキスパートとの協業重視
5. **Test-Driven Development**: テストが設計を駆動
6. **Clean Code**: 読みやすく、保守しやすいコード
7. **Validation as Code**: FluentValidationによる宣言的バリデーション

これらの原則に従うことで、GitHub Copilotがより適切なコード提案を行い、長期的に保守可能でビジネス要求に柔軟に対応できるアプリケーションを構築できます。

## GitHub Copilot との協業での要点

### DDDコンテキストでのコメント
```csharp
// TODO: 商品の価格変更ドメインサービス
// - 価格履歴の記録
// - 割引適用のビジネスルール
// - 在庫価値への影響計算
// Money値オブジェクトを活用した型安全な価格操作
```

### 値オブジェクト指向の実装要求
```csharp
// TODO: 注文数量の検証
// - StockQuantity値オブジェクトで在庫確認
// - 最小・最大注文数のビジネスルール適用
// - 値オブジェクトメソッドで不正状態の防止
```

これにより、GitHub Copilotはドメイン駆動設計と値オブジェクトの原則に沿った、より適切なコード提案を行います。