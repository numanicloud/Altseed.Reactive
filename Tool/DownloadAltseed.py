import urllib.request
import os
import shutil
import zipfile
import sys

def download(url, dst):
	any_url_obj = urllib.request.urlopen(url)
	local = open(dst, 'wb')
	local.write(any_url_obj.read())
	local.close()
	any_url_obj.close()

def unzip(zip_filename):
	zip_file = zipfile.ZipFile(zip_filename, "r")
	for f in zip_file.namelist():
		if not os.path.basename(f):
			os.makedirs(f)
		else:
			version = sys.version_info
			if version[0] == 2:
				unzip_file = file(f, "wb")
				unzip_file.write(zip_file.read(f))
				unzip_file.close()
			elif version[0] == 3:
				unzip_file = open(f, "wb")
				unzip_file.write(zip_file.read(f))
				unzip_file.close();
	zip_file.close()

def rmdir(path):
	if os.path.exists(path):
		print("rmdir : " + path)
		shutil.rmtree(path)
	else:
		print("rmdir : not found " + path)
	
if __name__ == "__main__":
	os.chdir(os.path.dirname(os.path.abspath(__file__)))
	os.chdir("../Lib")
	
	dirname = "Altseed_CS_20150906_WIN"
	rmdir(dirname)
	
	print('Altseedをダウンロードします')
	download("https://github.com/altseed/Altseed/releases/download/20150906/" + dirname + ".zip", "Altseed.zip")
	
	print('展開します')
	unzip('Altseed.zip')
	
	print('ファイルをコピーします')
	shutil.copy(dirname + '/Runtime/Altseed.dll', '../Dev/Nac.Altseed/Altseed.dll')
	shutil.copy(dirname + '/Runtime/Altseed.xml', '../Dev/Nac.Altseed/Altseed.xml')
	shutil.copy(dirname + '/Runtime/Altseed_core.dll', '../Dev/Nac.Altseed/Altseed_core.dll')
	
	shutil.copy(dirname + '/Runtime/Altseed.dll', '../Dev/Nac.Altseed.Reactive/Altseed.dll')
	shutil.copy(dirname + '/Runtime/Altseed.xml', '../Dev/Nac.Altseed.Reactive/Altseed.xml')
	shutil.copy(dirname + '/Runtime/Altseed_core.dll', '../Dev/Nac.Altseed.Reactive/Altseed_core.dll')

	shutil.copy(dirname + '/Runtime/Altseed.dll', '../Dev/Nac.Altseed.Test/Altseed.dll')
	shutil.copy(dirname + '/Runtime/Altseed.xml', '../Dev/Nac.Altseed.Test/Altseed.xml')
	shutil.copy(dirname + '/Runtime/Altseed_core.dll', '../Dev/Nac.Altseed.Test/Altseed_core.dll')
	
	print('完了')
	