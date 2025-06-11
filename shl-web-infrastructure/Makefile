fmt:
	terraform fmt -recursive

test:
	cd envs/test && terraform init && terraform plan

test_deploy:
	cd envs/test && terraform init && terraform apply

acc:
	cd envs/acc && terraform init && terraform plan

acc_deploy:
	cd envs/acc && terraform init && terraform apply

prd:
	cd envs/prd && terraform init && terraform plan

prd_deploy:
	cd envs/prd && terraform init && terraform apply