import os

def replace_content_in_file(file_path, old_str, new_str):
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
    except UnicodeDecodeError:
        return # Skip binary files

    if old_str in content:
        new_content = content.replace(old_str, new_str)
        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f"Updated content: {file_path}")

def process_directory(root_dir, old_str, new_str):
    # 1. Content Replacement
    for dirpath, dirnames, filenames in os.walk(root_dir):
        # Filter out ignored directories
        dirnames[:] = [d for d in dirnames if d not in ['.git', '.vs', 'bin', 'obj', '.github']]
        
        for filename in filenames:
            file_path = os.path.join(dirpath, filename)
            replace_content_in_file(file_path, old_str, new_str)

    # 2. Rename Files
    for dirpath, dirnames, filenames in os.walk(root_dir):
        dirnames[:] = [d for d in dirnames if d not in ['.git', '.vs', 'bin', 'obj']]
        
        for filename in filenames:
            if old_str in filename:
                old_file_path = os.path.join(dirpath, filename)
                new_filename = filename.replace(old_str, new_str)
                new_file_path = os.path.join(dirpath, new_filename)
                os.rename(old_file_path, new_file_path)
                print(f"Renamed file: {old_file_path} -> {new_file_path}")

    # 3. Rename Directories (Bottom-up)
    for dirpath, dirnames, filenames in os.walk(root_dir, topdown=False):
        for dirname in dirnames:
            if old_str in dirname:
                old_dir_path = os.path.join(dirpath, dirname)
                new_dirname = dirname.replace(old_str, new_str)
                new_dir_path = os.path.join(dirpath, new_dirname)
                # Ensure we don't overwrite existing
                if not os.path.exists(new_dir_path):
                    os.rename(old_dir_path, new_dir_path)
                    print(f"Renamed directory: {old_dir_path} -> {new_dir_path}")

if __name__ == "__main__":
    process_directory(r"d:\Backend", "N-Tier", "BackendSWP391")
    # Also replace N_Tier with BackendSWP391 for usages where underscore was used (namespaces often match folder structure but sometimes sanitized)
    # Actually, usually dots are used in namespaces "N_Tier" might be "N_Tier"
    # Let's check if N_Tier is used. The previous file view showed "using N_Tier.Application..."
    # So "N_Tier" (underscore) is definitely used in code.
    # The user request is "BackendSWP391", which doesn't have special chars.
    # So "N_Tier" -> "BackendSWP391" as well.
    process_directory(r"d:\Backend", "N_Tier", "BackendSWP391")
