namespace Challenge.IntegrationTest.Fixtures;

[CollectionDefinition(nameof(DatabaseCollection))]
public class DatabaseCollection : ICollectionFixture<PostgressContainerFixture>
{
}