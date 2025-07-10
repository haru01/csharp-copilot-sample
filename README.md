# C# Backend with Entity Framework and xUnit - GitHub Copilot Development Sample

このプロジェクトは、GitHub Copilotを活用したC#バックエンド開発のサンプルです。Entity Framework CoreとxUnitを使用し、TDD（テスト駆動開発）のアプローチでAPIを構築しています。

## 🚀 技術スタック

- **言語**: C# (.NET 9)
- **フレームワーク**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **データベース**: SQLite (開発用) / InMemory (テスト用)
- **テスト**: xUnit, Entity Framework InMemory
- **その他**: OpenAPI

## 📁 プロジェクト構造

```
csharp-copilot/
├── src/
│   └── CopilotSample.Api/          # Web APIプロジェクト
│       ├── Controllers/             # APIコントローラー
│       │   └── ProductsController.cs
│       ├── Models/                  # ドメインモデル
│       │   ├── Product.cs           # 商品エンティティ
│       │   └── Category.cs          # カテゴリエンティティ
│       ├── Data/                    # Entity Framework関連
│       │   ├── AppDbContext.cs      # データベースコンテキスト
│       │   ├── IProductRepository.cs# リポジトリインターフェース
│       │   └── ProductRepository.cs # リポジトリ実装
│       ├── Services/                # ビジネスロジック
│       │   ├── IProductService.cs   # サービスインターフェース
│       │   └── ProductService.cs    # サービス実装（CRUD + 在庫管理）
│       ├── Dtos/                    # データ転送オブジェクト
│       │   └── CreateProductDto.cs  # 商品作成用DTO
│       ├── Program.cs               # アプリケーションエントリポイント
│       └── CopilotSample.Api.http   # HTTPリクエストテンプレート
├── tests/
│   └── CopilotSample.Tests/        # xUnitテストプロジェクト
│       ├── Repositories/            # リポジトリテスト
│       │   └── ProductRepositoryTests.cs
│       ├── Services/                # サービステスト
│       │   └── ProductServiceTests.cs
│       └── Helpers/                 # テストヘルパー
│           ├── TestDbContextFactory.cs # インメモリDB設定
│           ├── ProductBuilder.cs    # 商品テストデータビルダー
│           └── CategoryBuilder.cs   # カテゴリテストデータビルダー
├── docs/                           # ドキュメント
├── CopilotSample.sln               # ソリューションファイル
├── README.md                        # このファイル
└── CLAUDE.md                        # 開発者向け詳細ドキュメント
```

## ⚡ クイックスタート

### 前提条件
- .NET 9 SDK

### プロジェクトのセットアップ
```bash
# リポジトリをクローン
git clone <repository-url>
cd csharp-copilot

# 依存関係の復元
dotnet restore

# プロジェクトのビルド
dotnet build
```

### API起動
```bash
dotnet run --project src/CopilotSample.Api
```

API は http://localhost:5000 で起動します。

### テスト実行
```bash
dotnet test
```

## 🔧 実装されている機能

### ビジネスロジック
- **CRUD操作**: 商品の作成、読み取り、更新、削除
- **在庫管理**: 在庫チェック、在庫減算、低在庫商品の検索
- **検索機能**: 商品名・SKUでの大文字小文字を区別しない検索
- **在庫計算**: 総在庫価値の計算
- **バリデーション**: SKUの重複チェック、在庫数の妥当性検証

### アーキテクチャパターン
- **Repository Pattern**: データアクセスの抽象化
- **Service Layer Pattern**: ビジネスロジックの分離
- **DTO Pattern**: データ転送オブジェクト
- **Dependency Injection**: 疎結合な設計

## 📊 API エンドポイント

- `GET /api/products` - 全商品取得
- `GET /api/products/{id}` - ID指定で商品取得
- `POST /api/products` - 新規商品作成
- `PUT /api/products/{id}` - 商品更新
- `DELETE /api/products/{id}` - 商品削除

詳細なAPIドキュメントは起動後 `/swagger` でご確認ください。

## 🧪 テスト戦略

### テストカバレッジ
- **リポジトリテスト**: データアクセス層のテスト（8テスト）
  - 商品の取得・作成・更新・削除
  - SKU存在チェック
  - カテゴリを含む商品データの取得
- **サービステスト**: ビジネスロジックのテスト（12テスト）
  - 商品検索（名前・SKU）の大文字小文字非依存
  - 在庫管理（チェック・減算・低在庫検索）
  - 在庫総価値の計算
  - 無効なパラメータの処理

**合計**: 20テスト（すべてインメモリDBを使用）

### テストベストプラクティス

#### 1. Arrange-Act-Assert パターンの徹底

すべてのテストは明確な3段階構造を採用しています：

```csharp
[Fact]
public async Task 商品名で大文字小文字を区別せずに検索できる()
{
    // Arrange - テストデータの準備
    var targetName = "Gaming Laptop"; // 検索対象の商品名
    var category = CategoryBuilder.Create();
    var matchingProduct = ProductBuilder.Create()
        .WithName(targetName) // 検索にマッチする商品
        .WithCategory(category);
    var nonMatchingProduct = ProductBuilder.Create()
        .WithName("Different Product") // マッチしない商品
        .WithCategory(category);
    
    _context.Categories.Add(category);
    _context.Products.AddRange(matchingProduct, nonMatchingProduct);
    await _context.SaveChangesAsync();
    
    // Act - テスト対象の実行
    var results = await _service.SearchProductsAsync("gaming laptop");
    
    // Assert - 結果の検証
    var products = results.ToList();
    Assert.Single(products);
    Assert.Equal(targetName, products[0].Name);
}
```

#### 2. TestDataBuilder パターンの活用

デフォルト値の差分のみを指定することで、テストの可読性を向上させています：

```csharp
// ProductBuilder - fluent interfaceでテストデータを構築
var product = ProductBuilder.Create()
    .WithName("Gaming Laptop")        // 差分のみ指定
    .WithPrice(1500.00m)             // 差分のみ指定
    .WithStockQuantity(10)           // 差分のみ指定
    .WithCategory(category);         // 差分のみ指定
// その他の属性（SKU、CreatedAt等）はデフォルト値を使用
```

#### 3. テストコンテキストの分離

各テストメソッドで独立したDbContextを使用し、テスト間の干渉を防いでいます：

```csharp
public class ProductRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        // 各テストメソッドごとに新しいインメモリDBを作成
        _context = TestDbContextFactory.CreateDbContext();
        _repository = new ProductRepository(_context);
    }

    public void Dispose()
    {
        // テスト終了時にコンテキストを破棄
        _context.Dispose();
    }
}
```

**分離のメリット**:
- テスト実行順序に依存しない
- 並列実行が可能
- テストデータの状態が他のテストに影響しない
- デバッグが容易

## 🤖 GitHub Copilot活用のポイント

### 1. コメントベースの開発
```csharp
// Test: Should return all products with categories
// Test: Should return product by ID with category
```

### 2. パターンの一貫性
- Builder パターンによるテストデータの一貫した作成
- Arrange-Act-Assert パターンの徹底

### 3. 段階的な実装
- モデル → リポジトリ → サービス → コントローラーの順
- テスト → 実装 → リファクタリングのサイクル

### 4. テスト駆動開発
- 日本語のテストメソッド名で意図を明確に表現
- Theory/InlineData を活用した複数パターンのテスト

## 🏗️ アーキテクチャの特徴

1. **レイヤー分離**: Controller → Service → Repository → DbContext
2. **依存性注入**: すべてのサービスはDIコンテナで管理
3. **テスタビリティ**: インメモリDBを使用した高速なテスト実行
4. **エラーハンドリング**: サービス層で例外を投げ、コントローラーで適切なHTTPレスポンスに変換

## 📈 今後の拡張案

- 認証・認可の実装（JWT）
- ロギングの追加（Serilog）
- API バージョニング
- キャッシングの実装
- バックグラウンドジョブ（Hangfire）
- GraphQL対応

## 📚 ドキュメント

詳細な開発者向けドキュメントは [CLAUDE.md](./CLAUDE.md) をご覧ください。

## 🤝 コントリビューション

プルリクエストや Issues はいつでも歓迎します。詳細な開発ガイドラインについては [CLAUDE.md](./CLAUDE.md) をご参照ください。