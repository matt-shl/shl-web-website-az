# Terraform Standard

This repository holds an default setup for Terraform. The standard is cloud agnostic and does not contain any defaults for the cloud providers.

## Requirements

- TFenv

## How to use

1. Checkout the repository
2. Copy over the files to the new project using the following commands:

    export NEW_PATH=/Volumes/Data/projects/veh/dtnl-veh-infrastructure-cloudflare

    cp -r ./* $NEW_PATH

    cp -r ./.gitignore $NEW_PATH

    cp -r ./.terraform-version $NEW_PATH

    cp -r ./.terraformignore $NEW_PATH


3. Change client workspace names in envs/*/main.tf