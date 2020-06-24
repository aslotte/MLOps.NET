using MLOps.NET.Storage;

namespace MLOps.NET.AWS
{
    /// <summary>
    /// Extension methods to allow the usage of Azure storage
    /// </summary>
    public static class MLOpsBuilderExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static MLOpsBuilder UseDynamoDBStorage(this MLOpsBuilder builder)
        {
            builder.UseMetaDataStore(new StorageAccountMetaDataStore());
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="awsAccessKeyId"></param>
        /// <param name="awsSecretAccessKey"></param>
        /// <param name="regionName"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public static MLOpsBuilder UseAWSS3Repository(this MLOpsBuilder builder, string awsAccessKeyId, string awsSecretAccessKey, string regionName, string bucketName)
        {
            builder.UseModelRepository(new StorageAccountModelRepository(awsAccessKeyId, awsSecretAccessKey, regionName, bucketName));

            return builder;
        }
    }
}