#!/bin/bash

# Remote deployment script - run this from your local machine
# It will SSH to your Azure VM and deploy the application

# Configuration - UPDATE THESE VALUES
VM_USER="azureuser"
VM_IP="YOUR_VM_IP_HERE"  # Replace with your actual VM IP

VM_SSH_KEY=""  # Optional: path to SSH key file

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}🚀 Starting remote deployment to Azure VM...${NC}"

# Check if VM_IP is set
if [ "$VM_IP" = "YOUR_VM_IP_HERE" ]; then
    echo -e "${RED}❌ Please update the VM_IP variable in this script with your actual VM IP address${NC}"
    exit 1
fi

# Build SSH command
SSH_CMD="ssh"
if [ ! -z "$VM_SSH_KEY" ]; then
    SSH_CMD="$SSH_CMD -i $VM_SSH_KEY"
fi
SSH_CMD="$SSH_CMD $VM_USER@$VM_IP"

echo -e "${YELLOW}📡 Connecting to Azure VM ($VM_USER@$VM_IP)...${NC}"

# Run deployment commands on the VM
$SSH_CMD << 'EOF'
echo "📥 Downloading latest deployment script..."
curl -o ~/deploy-azure.sh https://raw.githubusercontent.com/Dogrammer/temporal-poc/master/deploy-azure.sh
chmod +x ~/deploy-azure.sh

echo "🚀 Running deployment script..."
~/deploy-azure.sh

echo ""
echo "✅ Deployment completed on VM!"
EOF

if [ $? -eq 0 ]; then
    echo -e "${GREEN}🎉 Remote deployment successful!${NC}"
    echo ""
    echo -e "${YELLOW}📍 Your services should now be available at:${NC}"
    echo "   Web API: http://$VM_IP:5044"
    echo "   Swagger: http://$VM_IP:5044/swagger"
    echo "   Temporal UI: http://$VM_IP:8081"
    echo "   pgAdmin: http://$VM_IP:8080"
    echo ""
    echo -e "${YELLOW}📝 Test with:${NC}"
    echo "   curl -X POST http://$VM_IP:5044/api/workflow/start-loan \\"
    echo "     -H 'Content-Type: application/json' \\"
    echo "     -d '{\"loanId\": \"test123\"}'"
else
    echo -e "${RED}❌ Remote deployment failed!${NC}"
    exit 1
fi
