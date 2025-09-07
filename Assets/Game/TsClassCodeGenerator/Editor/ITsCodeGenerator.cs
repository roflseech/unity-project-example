namespace Game.TsClassCodeGenerator.Editor
{
    public interface ITsCodeGenerator
    {
        CodeGenerationReport GenerateCode(ITsParser parser, IMetaDataProvider metaDataProvider);
    }
}