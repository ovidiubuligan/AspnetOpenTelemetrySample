# cluster creation
# it installs traefik by default , disable it
```bash
k3d cluster create ovi-cluster --api-port 0.0.0.0:45845 --servers 1 --agents 3  --port 9999:80@loadbalancer --k3s-server-arg  '--disable=traefik' 
```
# Install helm3 if not already installed
```bash
➜  .k3d curl -fsSL -o get_helm.sh https://raw.githubusercontent.com/helm/helm/master/scripts/get-helm-3
➜  .k3d chmod 700 get_helm.sh
➜  .k3d ./get_helm.sh
```

# If you don't speciffy an api port then a new api port will be assigned each time the cluster restarts and you will need to do 
``
export KUBECONFIG=$(k3d kubeconfig write ovi-cluster)
``

# Inside cluster
# #######################################

Prometheus operator 
```bash
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo add stable https://charts.helm.sh/stable
helm repo update

kubectl create namespace monitoring
helm install prom --namespace=monitoring prometheus-community/kube-prometheus-stack

# get password for grafana
kubectl get secret --namespace monitoring prom-grafana -o jsonpath="{.data.admin-password}" | base64 --decode ; echo
```
edit etc/hosts file with :
```
# ###### local k3d kubernetes
192.168.0.108 jaeger.localhost  
192.168.0.108 alertmanager.localhost  
192.168.0.108 prometheus.localhost  
192.168.0.108 grafana.localhost  

``` 

# install nginix
```bash
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm install ovi-ingres  ingress-nginx/ingress-nginx 
```

# install certmanager
```bash
k create namespace cert-manager
helm repo add jetstack https://charts.jetstack.io
helm repo update

helm install \
  cert-manager jetstack/cert-manager \
  --namespace cert-manager \
  --version v1.0.4 \
   --set installCRDs=true
```
# add otel operator
```bash
kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml
```


# add a service
```bash
cat <<EOF | kubectl apply -f -
apiVersion: apps/v1
kind: Deployment
metadata:
  labels:
    app: nginx-app
  name: nginx-app
  namespace: default
spec:
  progressDeadlineSeconds: 600
  replicas: 1
  revisionHistoryLimit: 10
  selector:
    matchLabels:
      app: nginx-app
  strategy:
    rollingUpdate:
      maxSurge: 25%
      maxUnavailable: 25%
    type: RollingUpdate
  template:
    metadata:
      labels:
        app: nginx-app
    spec:
      containers:
      - image: nginx
        imagePullPolicy: Always
        name: nginx
        resources: {}
        terminationMessagePath: /dev/termination-log
        terminationMessagePolicy: File
      restartPolicy: Always
      schedulerName: default-scheduler
---
apiVersion: v1
kind: Service
metadata:
  labels:
    app: nginx-app-svc
  name: nginx-app-svc
spec:
  ports:
  - name: 80-80
    port: 80
    protocol: TCP
    targetPort: 80
  selector:
    app: nginx-app
  sessionAffinity: None
  type: ClusterIP
EOF
```

