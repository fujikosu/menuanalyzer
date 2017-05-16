from flask import render_template, request, jsonify, Flask
import http.client, urllib.request, urllib.parse, urllib.error, base64
import json
import requests

app = Flask(__name__)

@app.route("/")
def hello():
    return "Hello from an Azure Web App running on Linux!"

@app.route("/api/getlists", methods=["POST"])
def analyze():
    # Upload an image to blob storage
    if request.method == "POST":
        img = request.data

# Use Computer Vision API
        headers = {
            # Request headers
            # 'Content-Type': 'application/json',
            'Content-Type': 'application/octet-stream',
            'Ocp-Apim-Subscription-Key': SUBSCRIPTION_KEY,
        }

        params = urllib.parse.urlencode({
            # Request parameters
            'language': 'unk',
            'detectOrientation ': 'true',
        })

        res = requests.post(url='https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr', data=img, headers=headers, params=params)
        # print(res.json())
        return res.text

if __name__ == "__main__":
    app.run(host='0.0.0.0')