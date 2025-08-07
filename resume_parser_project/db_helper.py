import pyodbc

def insert_resume(data):
    conn_str = (
    "Driver={SQL Server};"
    "Server=localhost\\SQLEXPRESS02;"
    "Database=registration;"
    "UID=sa;"
    "PWD=123456;"
    "Encrypt=no;"
)

    with pyodbc.connect(conn_str) as conn:
        cursor = conn.cursor()
        cursor.execute("EXEC SP_Insert_ResumeInfo ?,?,?,?,?,?,?,?,?",
            data['Name'],
            data['Contact'],
            data['Address'],
            data['Skills'],
            data['Education'],
            data['Projects'],
            data['Publication'],
            data['Experience'],
            data['Hobbies']
        )
        conn.commit()
