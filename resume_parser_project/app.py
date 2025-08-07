from flask import Flask, request, jsonify
from flask_cors import CORS
import os
from parser import extract_text, extract_fields
from db_helper import insert_resume
import json
from flask import Response

app = Flask(__name__)
CORS(app)

@app.route('/upload', methods=['POST'])
def upload_resume():
    file = request.files.get("file")
    if not file:
        return "No file uploaded", 400

    upload_dir = "uploads"
    os.makedirs(upload_dir, exist_ok=True)
    file_path = os.path.join(upload_dir, file.filename)
    file.save(file_path)

    text = extract_text(file_path)
    parsed_data = extract_fields(text)
    insert_resume(parsed_data)

    os.remove(file_path)
    #return jsonify(parsed_data)
    return Response(json.dumps(parsed_data), mimetype="application/json")

if __name__ == '__main__':
    app.run(debug=True, port=5000)