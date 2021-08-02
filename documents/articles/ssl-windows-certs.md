# SSL Certificates - Windows

This article shows how to setup certificates on windows.

## cfssl

Download `cfssl` and `cfssljson` from the
[Releases](https://github.com/cloudflare/cfssl/releases)
page. I chose version 1.6.0.

* `cfssl_1.6.0_windows_amd64.exe`
* `cfssljson_1.6.0_windows_amd64.exe`

In file explorer rename the downloaded files to `cfssl.exe` and
`cfssljson.exe` respectively and move them to `C:\Windows\System32`.

## Create the Certificate Authority

To create a self signed certificate authority for a company called "JetBlack" based in London, England, Great Britain, create the following config file “ca.json”.

```json
{
  "CN": "JetBlack Root CA",
  "key": {
    "algo": "rsa",
    "size": 2048
  },
  "names": [
  {
    "C": "GB",
    "L": "London",
    "O": "JetBlack",
    "OU": "JetBlack Root CA",
    "ST": "England"
  }
 ]
}
```

The following command creates "ca.pem" and "ca-key.pem".

```bash
cfssl gencert -initca ca.json | cfssljson -bare ca
```

## Create the Profiles

The next steps require a profile config file. The profile describes general details about the certificate. For example it’s duration, and usages.

Create the following file "cfssl.json".

```json
{
  "signing": {
    "default": {
      "expiry": "8760h"
    },
    "profiles": {
      "intermediate_ca": {
        "usages": [
            "signing",
            "digital signature",
            "key encipherment",
            "cert sign",
            "crl sign",
            "server auth",
            "client auth"
        ],
        "expiry": "8760h",
        "ca_constraint": {
            "is_ca": true,
            "max_path_len": 0, 
            "max_path_len_zero": true
        }
      },
      "peer": {
        "usages": [
            "signing",
            "digital signature",
            "key encipherment", 
            "client auth",
            "server auth"
        ],
        "expiry": "8760h"
      },
      "server": {
        "usages": [
          "signing",
          "digital signing",
          "key encipherment",
          "server auth"
        ],
        "expiry": "8760h"
      },
      "client": {
        "usages": [
          "signing",
          "digital signature",
          "key encipherment", 
          "client auth"
        ],
        "expiry": "8760h"
      }
    }
  }
}
```

We can see how the "client" profile specifies "client auth" in its usages, while the "server" profile specifies "server auth".

## Create the Intermediate CA

To create an intermediate certificate authority create the following config file "intermediate-ca.json".

```json
{
  "CN": "JetBlack Intermediate CA",
  "key": {
    "algo": "rsa",
    "size": 2048
  },
  "names": [
    {
      "C":  "GB",
      "L":  "London",
      "O":  "JetBlack",
      "OU": "JetBlack Intermediate CA",
      "ST": "England"
    }
  ],
  "ca": {
    "expiry": "42720h"
  }
}
```

The following commands creates "intermediate_ca.pem", "intermediate_ca.csr" and "intermediate_ca-key.pem" and signs the certificate.

```bash
cfssl gencert -initca intermediate-ca.json | cfssljson -bare intermediate_ca
cfssl sign -ca ca.pem -ca-key ca-key.pem -config cfssl.json -profile intermediate_ca intermediate_ca.csr | cfssljson -bare intermediate_ca
```

Note the second "sign" command uses the CA produced previously to sign the intermediate CA. It also uses the "cfssl.json" profile and specifies the "intermediate_ca" profile.

## Creating the Host Certificates

The fully qualified domain name of my machine is `windowsvm.jetblack.net`.
Here is an example host certificate config file "host.json".

```json
{
  "CN": "windowsvm.jetblack.net",
  "key": {
    "algo": "rsa",
    "size": 2048
  },
  "names": [
  {
    "C": "GB",
    "L": "London",
    "O": "JetBlack",
    "OU": "JetBlack Hosts",
    "ST": "England"
  }
  ],
  "hosts": [
    "windowsvm.jetblack.net",
    "localhost"
  ]
}
```

To generate the certificates with the above config do the following:

```bash
cfssl gencert -ca intermediate_ca.pem -ca-key intermediate_ca-key.pem -config cfssl.json -profile=peer host.json | cfssljson -bare peer
cfssl gencert -ca intermediate_ca.pem -ca-key intermediate_ca-key.pem -config cfssl.json -profile=server host.json | cfssljson -bare server
cfssl gencert -ca intermediate_ca.pem -ca-key intermediate_ca-key.pem -config cfssl.json -profile=client host.json | cfssljson -bare client
```

Now copy all the `.pem` files to a folder under your home folder called `.keys`.
Rename the keys to be `.key` (e.g. rename `ca-key.pem` to `ca.key`) and the certificates
to `.crt` (e.g. rename `ca.pem` to `ca.crt`).

We need to make a pkcs12 file for the server certificate. Download the openssl toolkit
from [here](https://slproweb.com/products/Win32OpenSSL.html).
I used "Win64 OpenSSL v1.1.1k Light". The following command will make the pkcs12 file.
You will be prompted for a password. I entered a password, but I need to check if en
empty password is sufficient.

```bash
openssl pkcs12 -export -inkey server.key -in server.crt -name 'JetBlack Server' -out server.pfx
```

Now open the microsoft management console. Click on `File` and choose `Add/Remove Snap-in`.
Select `Certificates` and click `Add`. Choose `Computer account` to manage the certificates
and click `Next`. Select `Local Computer` (the default) and click `Finish`. The certificates
snap-in has been selected, now click `OK`.

From `Console Root` expand `Certificates`, `Trusted Root Certification Authorities`,
and `Certificates`. Right click on `All Tasks` and select `Import...`. Click
through the wizard and select `ca.crt` and complete. Next import `intermediate-ca.crt`
into the `Intermediate Certification Authorities`.

Finally import the `server.pfx` into the `Personal` key store.
