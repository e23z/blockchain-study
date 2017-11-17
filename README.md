# Blockchain Study

This is just a simple blockchain implemented with C# and .NET Core for study purposes.

---

## Requirements

* .Net Core 2.0

---

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

First of all you need to install .NET Core into your machine. This is very simple, just go to Microsoft's [.NET Core page](https://www.microsoft.com/net/core) and follow the instructions for your platform.

### Installing

Just clone it to your computer, then restore the dependencies and run!

```
git clone git@github.com:feuerwelt/blockchain-study.git <yourfolder>
cd <yourfolder>
dotnet restore
dotnet run --server.urls http://0.0.0.0:5000
```

Also, you'll need since we need more than one instance to test the blockchain synchronization. So, open another terminal/console window and run the following command to start a new instance of this blockchain api on another port.

```
dotnet run --server.urls http://0.0.0.0:5001
```

End with an example of getting some data out of the system or using it for a little demo

---
## API

If you have a PAW license, you can use the file `API Test.paw` to test the blockchain API. Otherwise, you can use cURL or any other tool of your liking to interact with the API.

### GET `/api/chain`

  Get the current chain state.

  **cURL**
  ```
  curl "http://localhost:5000/api/chain"
  ```

  **Response**

  ```
  {
   "chain": [
      {
        "index": 1,
        "timestamp": 1510858056,
        "transactions": [
          {  
            "sender": "D8F468575237406AB52F475752F0D004",
            "recipient": "FE10A5D23EA44C15A6E104C6254CF1B2",
            "amount": 2
          }
        ],
        "proof": 1,
        "prevHash": "100"
      }
   ],
   "length": 1
  }
  ```

### POST `/api/transactions/new`

Generates a new transaction with the values posted.

**Request**
```
{
  "sender": "<string>",
  "recipient": "<string>",
  "amount": "<int>"
}
```

**Response**
```
{
  "message": "Transaction will be added to Block 2"
}
```

**cURL**
```
curl -X "POST" "http://localhost:5000/api/transactions/new" \
     -H 'Content-Type: application/json; charset=utf-8' \
     -d $'{
  "amount": "2",
  "recipient": "def",
  "sender": "abc"
}'
```

### GET `/api/mine`

Request for the current block to be closed, validated and added to the chain. It also rewards the current node with a "mining coin".

**Response**

A status message alongside with the transactions, the block id, its proof and hash.

```
{
  "message":"New Block Forged",
  "index":2,
  "transactions":[
    {
      "sender":"0",
      "recipient":"45f46fbf78ef4cc58da4779c4c800c42",
      "amount":1
    }
  ],
  "proof":72608,
  "prevHash":"A07ADB1E9D6B68D1820F61098CE82F15081F9E44D763038EEFEF73B9495E915F"
}
```

**cURL**
```
curl "http://localhost:5000/api/mine"
```

### GET `/api/nodes`

Get the nodes registered in this blockchain.


**Response**

A list with the urls of the nodes registered and its length, plus the current node url.
```
{
  "nodes": [
    "http://localhost:5000"
  ],
  "length": 1
}
```

**cURL**
```
curl "http://localhost:5000/api/nodes"
```

### POST `/api/nodes/register`

Register one or many nodes in the blockchain.

**Request**
```
{
  "nodes": [
    "<url>"
  ]
}
```

**Response**

The status of the request and the updated list of nodes registered to the blockchain.
```
{
  "message":"New nodes have been added",
  "totalNodes":[
    "http://localhost:5001"
  ]
}
```

**cURL**
```
curl -X "POST" "http://localhost:5000/api/nodes/register" \
     -H 'Content-Type: application/json; charset=utf-8' \
     -d $'{
  "nodes": [
    "http://localhost:5001"
  ]
}'
```

### GET `/api/nodes/resolve`

Check all other blockchains registered and updates this one, if it's outdated.

**Response**

A status message and the updated chain state.

```
{
  "message":"Our chain was replaced",
  "newChain":[
    {
      "index":1,
      "timestamp":1510854876,
      "transactions":[

      ],
      "proof":1,
      "prevHash":"100"
    },
    {
      "index":2,
      "timestamp":1510854882,
      "transactions":[
        {
          "sender":"abc",
          "recipient":"def",
          "amount":2
        },
        {
          "sender":"0",
          "recipient":"2c1d8bd56cd542ddbfe3e79b9a6100f7",
          "amount":1
        }
      ],
      "proof":72608,
      "prevHash":"B283D02573C15D555CE462A718D58E015110505CCD2E51A37F4DC2D653DDB96C"
    },
    {
      "index":3,
      "timestamp":1510854907,
      "transactions":[
        {
          "sender":"abc",
          "recipient":"def",
          "amount":2
        },
        {
          "sender":"abc",
          "recipient":"def",
          "amount":2
        },
        {
          "sender":"0",
          "recipient":"2c1d8bd56cd542ddbfe3e79b9a6100f7",
          "amount":1
        }
      ],
      "proof":24348,
      "prevHash":"1E5EF08C7BAD15CAA589296B2B91DD6205AE3AE9AF0A8E762D4ABDB75C4F947D"
    }
  ]
}
```

**cURL**
```
curl "http://localhost:5000/api/nodes/resolve"
```

---
## Built With

* [.NET Core](https://www.microsoft.com/net/core) - Web Framework
---
## Authors

* **Edio Zalewski** - *Initial work* - [feuerwelt](https://github.com/feuerwelt)
---
## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

---
## Acknowledgments

Thanks a lot to Daniel van Flymen and his fantastic explanation of blockchain inner workings. [See more here](https://hackernoon.com/learn-blockchains-by-building-one-117428612f46)
