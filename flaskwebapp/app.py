from flask import render_template, request, jsonify, Flask
import http.client, urllib.request, urllib.parse, urllib.error, base64
import json
import requests
import os

app = Flask(__name__)

@app.route("/")
def hello():
    return "Hello from an Azure Web App running on Linux!"

@app.route("/api/getlists", methods=["POST"])
def getlists():
    # Upload an image to blob storage
    if request.method == "POST":
        img = request.data
        list_of_menuname = ocr_into_list_of_menu_name(img)
        json_of_name_and_imageURL = get_set_name_image_url(list_of_menuname)
        return json.dumps(json_of_name_and_imageURL)


@app.route("/api/getlistsmultipart", methods=["POST"])
def getlistsmultipart():
    # Upload an image to blob storage
    if request.method == "POST":
        if 'file' not in request.files:
            return 'No file part'
        # if user does not select file, browser also
        # submit a empty part without filename
        if file.filename == '':
            return 'No selected file'
        # if file:
        #  and allowed_file(file.filename):
        #     filename = secure_filename(file.filename)
        upload_file = request.files['file']        
        img = upload_file.read()
        list_of_menuname = ocr_into_list_of_menu_name(img)
        json_of_name_and_imageURL = get_set_name_image_url(list_of_menuname)
        return json.dumps(json_of_name_and_imageURL)

def ocr_into_list_of_menu_name(img):
    # Use Computer Vision API
    headers = {
        # Request headers
        'Content-Type': 'application/octet-stream',
        'Ocp-Apim-Subscription-Key': os.getenv('SUBSCRIPTION_KEY_OCR'),
        # 'Ocp-Apim-Subscription-Key': SUBSCRIPTION_KEY_OCR,
    }

    params = urllib.parse.urlencode({
        # Request parameters
        'language': 'unk',
        'detectOrientation ': 'true',
    })

    res = requests.post(url='https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr', data=img, headers=headers, params=params)
    # debug_file = open("debug_file.txt","w") 
    # debug_file.write(res.text) 
    # debug_file.close()

    list_of_menuname = parse_to_list_of_menuname(res.json())
    return list_of_menuname

def parse_to_list_of_menuname(ocr_json):
    lines = ocr_json["regions"][0]["lines"]
    list_of_menuname = []
    for line in lines:
        menu = ""
        for word in line["words"]:
            menu += word["text"] + " "
        list_of_menuname.append(menu)
    return list_of_menuname


def get_set_name_image_url(list_of_menuname):
    endpoint = 'https://api.cognitive.microsoft.com/bing/v5.0/images/search'
    headers = { 'Ocp-Apim-Subscription-Key': os.getenv('SUBSCRIPTION_KEY_IMAGE_SEARCH') }
    # headers = { 'Ocp-Apim-Subscription-Key': SUBSCRIPTION_KEY_IMAGE_SEARCH }
    menu_image_set = {"menus": []}
    for query in list_of_menuname:
        params = {
            'q': query,
            # 'mkt': 'ja-JP',
            'count': 1,
        }
        response = requests.get(endpoint, headers=headers, params=params)
        for result in response.json()['value']:
            menu_image_set["menus"].append({"name":query, "contentURL":result['contentUrl']})
    return menu_image_set

if __name__ == "__main__":
    app.run(host='0.0.0.0')