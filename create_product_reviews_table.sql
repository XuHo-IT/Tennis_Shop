-- Create product_reviews table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'product_reviews')
BEGIN
    CREATE TABLE [dbo].[product_reviews] (
        [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [product_id] INT NOT NULL,
        [user_id] INT NOT NULL,
        [full_name] NVARCHAR(100) NULL,
        [rating] INT NOT NULL,
        [comment] NVARCHAR(1000) NULL,
        [created_at] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK__product_reviews__product_id] 
            FOREIGN KEY ([product_id]) REFERENCES [dbo].[products]([id]) ON DELETE CASCADE,
        CONSTRAINT [FK__product_reviews__user_id] 
            FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_product_reviews_product_id] ON [dbo].[product_reviews]([product_id]);
    CREATE INDEX [IX_product_reviews_user_id] ON [dbo].[product_reviews]([user_id]);
END
GO

