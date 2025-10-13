# Deploy to Azure VM (POC Setup)

## Prerequisites
- Azure VM with Ubuntu 22.04
- SSH access to the VM
- Ports 5044, 7233, 8081 open in Azure Network Security Group

## Quick Setup

### 1. Open Firewall Ports in Azure Portal
Go to: **VM → Networking → Inbound port rules**

Add these rules:
- **Port 5044** (Web API) - Allow from Any
- **Port 7233** (Temporal) - Allow from Any  
- **Port 8081** (Temporal UI) - Allow from Any

### 2. Deploy from GitHub (Recommended)
```bash
# SSH into your VM and run the deployment script
ssh azureuser@<YOUR_VM_IP>
# The deploy script will automatically clone from GitHub
```

### 3. Install Docker (first time only)
```bash
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER
newgrp docker
```

### 4. Install Docker Compose (first time only)
```bash
sudo apt-get update
sudo apt-get install -y docker-compose-plugin
```

### 5. Install .NET 8 (optional - only needed if you want to run Web API outside Docker)
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc
```

### 6. Deploy the Application
```bash
# Download and run the deployment script
curl -o ~/deploy-azure.sh https://raw.githubusercontent.com/Dogrammer/temporal-poc/master/deploy-azure.sh
chmod +x ~/deploy-azure.sh
~/deploy-azure.sh
```

## That's It! 🎉

Your services are now running and accessible from anywhere:

- **Web API**: `http://<YOUR_VM_IP>:5044`
- **Swagger UI**: `http://<YOUR_VM_IP>:5044/swagger`
- **Temporal UI**: `http://<YOUR_VM_IP>:8081`

## Test the API

```bash
# Start a workflow
curl -X POST http://<YOUR_VM_IP>:5044/api/workflow/start-loan \
  -H 'Content-Type: application/json' \
  -d '{"loanId": "test123"}'

# Get result
curl http://<YOUR_VM_IP>:5044/api/workflow/result/kredit-test123
```

## Useful Commands

```bash
# Check services status
docker compose ps

# View logs
docker compose logs -f worker
docker compose logs -f temporal

# View Web API logs
tail -f webapi.log

# Restart everything
docker compose restart

# Stop everything
docker compose down
pkill -f "dotnet.*TemporalWebApi"
```

## Updating Code

When you make changes:

```bash
# From local machine - commit and push changes
git add .
git commit -m "Your changes description"
git push origin master

# On VM - redeploy from GitHub
~/deploy-azure.sh
```

## Security Notes (For POC)
⚠️ This is a POC setup - NOT production ready!
- No HTTPS/SSL
- Default PostgreSQL password
- No authentication on endpoints
- All ports publicly accessible

For production, add:
- Nginx reverse proxy with SSL
- Azure Application Gateway
- Strong passwords
- Authentication/authorization
- Private networking

