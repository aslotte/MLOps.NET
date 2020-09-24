namespace MLOps.NET.Kubernetes.Settings
{
    /// <summary>
    /// Kubernetes settings
    /// </summary>
    public sealed class KubernetesSettings
    {
        /// <summary>
        /// Path to kubeconfig
        /// </summary>
        public string KubeConfigPath { get; set; }

        /// <summary>
        /// Name of image pull secret
        /// </summary>
        public string ImagePullSecretName => "mlopsnet";

        /// <summary>
        /// Name of deploy manifest
        /// </summary>
        public string DeployManifestName => "deploy.yml";

        /// <summary>
        /// Name of service manifest
        /// </summary>
        public string ServiceManifestName => "service.yml";
    }
}
