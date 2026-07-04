BEGIN TRANSACTION;
DROP INDEX [IX_ProductVariantAttribute_ProductVariantId] ON [dbo].[ProductVariantAttribute];

DROP INDEX [IX_AttributeValue_AttributeId] ON [dbo].[AttributeValue];

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[ProductVariantAttribute]') AND [c].[name] = N'CreatedDate');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [dbo].[ProductVariantAttribute] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [dbo].[ProductVariantAttribute] ALTER COLUMN [CreatedDate] datetime2 NULL;

ALTER TABLE [dbo].[ProductVariantAttribute] ADD [CreatedBy] int NULL;

ALTER TABLE [dbo].[ProductVariantAttribute] ADD [DeletedBy] int NULL;

ALTER TABLE [dbo].[ProductVariantAttribute] ADD [DeletedDate] datetime2 NULL;

ALTER TABLE [dbo].[ProductVariantAttribute] ADD [ModifiedBy] int NULL;

ALTER TABLE [dbo].[ProductVariantAttribute] ADD [ModifiedDate] datetime2 NULL;

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[AttributeValue]') AND [c].[name] = N'CreatedDate');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[AttributeValue] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [dbo].[AttributeValue] ALTER COLUMN [CreatedDate] datetime2 NULL;

ALTER TABLE [dbo].[AttributeValue] ADD [ColorHex] nvarchar(7) NULL;

ALTER TABLE [dbo].[AttributeValue] ADD [CreatedBy] int NULL;

ALTER TABLE [dbo].[AttributeValue] ADD [DeletedBy] int NULL;

ALTER TABLE [dbo].[AttributeValue] ADD [DeletedDate] datetime2 NULL;

ALTER TABLE [dbo].[AttributeValue] ADD [ModifiedBy] int NULL;

ALTER TABLE [dbo].[AttributeValue] ADD [ModifiedDate] datetime2 NULL;

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Attribute]') AND [c].[name] = N'ModifiedDate');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Attribute] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [dbo].[Attribute] ALTER COLUMN [ModifiedDate] datetime2 NULL;

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Attribute]') AND [c].[name] = N'ModifiedBy');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Attribute] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [dbo].[Attribute] ALTER COLUMN [ModifiedBy] int NULL;

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Attribute]') AND [c].[name] = N'DeletedDate');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Attribute] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [dbo].[Attribute] ALTER COLUMN [DeletedDate] datetime2 NULL;

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Attribute]') AND [c].[name] = N'DeletedBy');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Attribute] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [dbo].[Attribute] ALTER COLUMN [DeletedBy] int NULL;

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Attribute]') AND [c].[name] = N'CreatedDate');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Attribute] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [dbo].[Attribute] ALTER COLUMN [CreatedDate] datetime2 NULL;

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Attribute]') AND [c].[name] = N'CreatedBy');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Attribute] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [dbo].[Attribute] ALTER COLUMN [CreatedBy] int NULL;

ALTER TABLE [dbo].[Attribute] ADD [IsVariantDefining] bit NOT NULL DEFAULT CAST(0 AS bit);

CREATE TABLE [dbo].[CategoryAttribute] (
    [Id] int NOT NULL IDENTITY,
    [CategoryId] int NOT NULL,
    [AttributeId] int NOT NULL,
    [IsRequired] bit NOT NULL,
    [DisplayOrder] int NOT NULL,
    [CreatedDate] datetime2 NULL,
    [CreatedBy] int NULL,
    [ModifiedDate] datetime2 NULL,
    [ModifiedBy] int NULL,
    [DeletedDate] datetime2 NULL,
    [DeletedBy] int NULL,
    [Deleted] bit NOT NULL,
    CONSTRAINT [PK_CategoryAttribute] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CategoryAttribute_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [dbo].[Attribute] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CategoryAttribute_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[ProductAttribute] (
    [Id] int NOT NULL IDENTITY,
    [ProductId] int NOT NULL,
    [AttributeId] int NOT NULL,
    [AttributeValueId] int NULL,
    [CustomValue] nvarchar(255) NULL,
    [CreatedDate] datetime2 NULL,
    [CreatedBy] int NULL,
    [ModifiedDate] datetime2 NULL,
    [ModifiedBy] int NULL,
    [DeletedDate] datetime2 NULL,
    [DeletedBy] int NULL,
    [Deleted] bit NOT NULL,
    CONSTRAINT [PK_ProductAttribute] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_PA_ValueXor] CHECK ((AttributeValueId IS NOT NULL AND CustomValue IS NULL) OR (AttributeValueId IS NULL AND CustomValue IS NOT NULL)),
    CONSTRAINT [FK_ProductAttribute_AttributeValue_AttributeValueId] FOREIGN KEY ([AttributeValueId]) REFERENCES [dbo].[AttributeValue] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProductAttribute_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [dbo].[Attribute] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProductAttribute_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_ProductVariantAttribute_ProductVariantId_AttributeId] ON [dbo].[ProductVariantAttribute] ([ProductVariantId], [AttributeId]);

ALTER TABLE [dbo].[ProductVariantAttribute] ADD CONSTRAINT [CK_PVA_ValueXor] CHECK ((AttributeValueId IS NOT NULL AND CustomValue IS NULL) OR (AttributeValueId IS NULL AND CustomValue IS NOT NULL));

CREATE UNIQUE INDEX [IX_AttributeValue_AttributeId_Value] ON [dbo].[AttributeValue] ([AttributeId], [Value]);

ALTER TABLE [dbo].[Attribute] ADD CONSTRAINT [CK_Attribute_Type] CHECK (AttributeType IN ('Text', 'Number', 'Color', 'Boolean'));

CREATE INDEX [IX_CategoryAttribute_AttributeId] ON [dbo].[CategoryAttribute] ([AttributeId]);

CREATE UNIQUE INDEX [IX_CategoryAttribute_CategoryId_AttributeId] ON [dbo].[CategoryAttribute] ([CategoryId], [AttributeId]);

CREATE INDEX [IX_ProductAttribute_AttributeId] ON [dbo].[ProductAttribute] ([AttributeId]);

CREATE INDEX [IX_ProductAttribute_AttributeValueId] ON [dbo].[ProductAttribute] ([AttributeValueId]);

CREATE UNIQUE INDEX [IX_ProductAttribute_ProductId_AttributeId] ON [dbo].[ProductAttribute] ([ProductId], [AttributeId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260703020505_EAV_Schema_Refactor', N'9.0.13');

COMMIT;
GO

