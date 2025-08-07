import fitz  # PyMuPDF
import docx2txt
import re
from collections import OrderedDict

def extract_text(file_path):
    if file_path.endswith(".pdf"):
        doc = fitz.open(file_path)
        return "\n".join([page.get_text() for page in doc])
    elif file_path.endswith(".docx"):
        return docx2txt.process(file_path)
    return ""

def extract_section(text, section_name):
    sections = [
        "Skills", "Education", "Projects", "Publication", "Experience", "Hobbies",
        "Declaration", "Objective", "Internship", "Industrial Internship", "Others"
    ]

    start = re.search(fr"(?i){section_name}[:\s]*\n?", text)
    if not start:
        return ""

    start_idx = start.end()
    end_idx = len(text)

    for sec in sections:
        if sec.lower() == section_name.lower():
            continue
        match = re.search(fr"(?i)\n{sec}[:\s]*\n?", text[start_idx:])
        if match:
            end_idx = start_idx + match.start()
            break

    return text[start_idx:end_idx].strip()

def extract_name(text):
    lines = text.strip().splitlines()

    # Look near the top of the resume (first 10 lines)
    for line in lines[:10]:
        cleaned = line.strip().replace("•", "").replace(",", "").strip()

        # Skip lines with keywords like email, phone, linkedin, etc.
        if any(keyword in cleaned.lower() for keyword in ['email', 'phone', 'linkedin', 'github']):
            continue
        if re.match(r"^[A-Z][a-zA-Z]+(?:\s[A-Z][a-zA-Z]+)+$", cleaned):
            return cleaned

    return ""

#def extract_contact(text):
#    phone = re.search(r"\+?\d[\d\s\-()]{8,}", text)
#    email = re.search(r"[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+", text)
#    return ", ".join(filter(None, [phone.group(0) if phone else "", email.group(0) if email else ""]))

def extract_contact(text):
    phone = re.search(r"\+?\d[\d\s\-()]{8,}", text)
    return phone.group(0) if phone else ""

def extract_address(text):
    match = re.search(r"(?i)(Address|Location)[:\-]?\s*(.+?)(?:\n|$)", text)
    return match.group(2).strip() if match else ""

def extract_fields(text):
    return OrderedDict([
        ("Name", extract_name(text)),
        ("Contact", extract_contact(text)),
        ("Address", extract_address(text)),
        ("Skills", extract_section(text, "Skills")),
        ("Education", extract_section(text, "Education")),
        ("Projects", extract_section(text, "Projects")),
        ("Publication", extract_section(text, "Publication") or extract_section(text, "Published Paper")),
        ("Experience", extract_section(text, "Experience") or extract_section(text, "Industrial Internship")),
        ("Hobbies", extract_section(text, "Hobbies") or extract_section(text, "Hobbies and Interests"))
    ])
