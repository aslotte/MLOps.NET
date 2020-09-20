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
    }
}
